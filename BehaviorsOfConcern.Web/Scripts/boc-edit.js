
var prevailingReportingSchoolID = -1;  //holds the corrected ID, if present, or else the original reporting school ID
var schoolMap = {};  //assoc-array of school names indexed by school ID
var timeoutId, localMods = {}, postedMods = {}, origTextFieldBackgroundColor;
var apiBaseUri = sessionStorage.getItem('bocUri') || '';
var userName = sessionStorage.getItem('userName') || '';
var userSchoolID = Number(sessionStorage.getItem('userSchoolID') || '0');
var workingIncidentID = Number(sessionStorage.getItem('workingIncidentID') || '0');


//ready handler:
$(document).ready(function () {
    if (isNaN(workingIncidentID) || (workingIncidentID == 0)) {
        window.location.assign($('#UnknownIncident').val());  //abort if no valid workingIncidentID is available
        return;
    }

    $('[data-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });

    $.ajaxSetup({
        headers: {
            "Authorization": "Basic " + (sessionStorage.getItem('authToken') || '')
        }
    });

    $('#incidentID').text(workingIncidentID);
    $('#sessionUserName').text(userName || "");
    resetAssignStudentModal(null, true);
    pullIncidentAndPopulate(workingIncidentID);
    pullCommentsAndPopulate(workingIncidentID);

    //handle drop-down updates
    $('#incidentEditForm').on('change', 'select', function (event) {
        var formdata = $(this).closest('form')
            .serialize()
            .replace(/correctedReportingSchoolID=/gi, "correctedReportingSchool.ID=")
            .replace(/reportingParty=/gi, "reportingParty.Name=");

        pushIncidentUpdates(workingIncidentID, formdata);
    });

    //additional handling for corrected reporting school drop-down
    $('#correctedReportingSchoolID').on('change', function (event) {
        prevailingReportingSchoolID = $(this).val();
        schoolMap[prevailingReportingSchoolID] = $(this).find("option:selected").text();
        resetAssignStudentModal(prevailingReportingSchoolID, true);
    });

    //handle comment command
    $('#addCommentCommand').on('click', function (event) {
        pushComment(workingIncidentID, $('#comment').val());
    });

    //handle modal search command
    $('#searchStudentCommand').on('click', function (event) {
        pullStudentSearchResults(prevailingReportingSchoolID, $('#assignStudent-searchTerm').val());
    });

    ////handle modal cancel
    //$('#assignStudentCancelCommand').on('click', function (event) {
    //    resetAssignStudentModal(prevailingReportingSchoolID, true);
    //});
    //handle modal hidden event
    $('#assignStudentModal').on('hidden.bs.modal', function (e) {
        resetAssignStudentModal(prevailingReportingSchoolID, true);
    });


    //(dis/en)able modal 'assign...' button
    $('#assignStudent-matches').on('change', function (event) {
        var selectedStudentPersonID = $(this).val();
        $('#assignStudentCommand').prop('disabled', (selectedStudentPersonID <= 0));
    });

    //handle modal student assignment command
    $('#assignStudentCommand').on('click', function (event) {
        pushConcernedParty(workingIncidentID, $('#assignStudent-matches').val());
    });

    origTextFieldBackgroundColor = $('#specificLocation').css('backgroundColor');

    //handle user modifications to text fields
    $('#correctedIncidentDate, #specificLocation, #reportingParty').on('input', function (event) {
        //var $targetElement = $(this);
        markFieldForPersistence($(this));
    });
});
//end - ready handler



function markFieldForPersistence($targetElement) {
    // color element bk as a visual cue to user that mod. data is not yet persisted to server
    $targetElement.css({ backgroundColor: 'AliceBlue' });
    // set property on localMods object and set it equal to the current jQuery element
    localMods[$targetElement.attr('id')] = $targetElement;
    // if a timer was started, clear it because they are still pressing keys like a monkey
    if (timeoutId) clearTimeout(timeoutId);
    // start a timer that will fire save when finished
    timeoutId = setTimeout(pushTextModifications, 850);
}

function pushTextModifications() {
    var updates = {};

    // iterate over 'localMods' object and populate 'updates' with data from element
    for (var key in localMods) {
        var $textElement = localMods[key];  //get reference to element we stored on localMods object
        var elementValue = $textElement.val();
        $textElement.css({ backgroundColor: origTextFieldBackgroundColor });  //change color on text box to show it is being persisted

        if ((postedMods[key] == undefined) || (postedMods[key] != elementValue)) {
            //add to update list, assuming it's different from last persisted value (avoids spurious server round-trips for things like window lost-focus events, etc.)
            if ($textElement.attr('id') == 'correctedIncidentDate') {
                //add to updates if vaild date
                var y = (new Date(elementValue)).getFullYear();
                if ((elementValue.length >= 6) && !isNaN(y) && (y >= 1900)) updates[key] = elementValue;
            } else {
                if (elementValue.trim().length < 1) updates[key] = '\b';  //force server to write empty value (send backspace - server will recognize as empty value)
                else updates[key] = elementValue;
            }
        }
    }

    var incidentParams = $.param(updates).replace(/reportingParty=/gi, "reportingParty.Name=");

    if (incidentParams.length == 0) {
        localMods = {};
    } else {
        //console.log('REST PUT (#' + workingIncidentID + '): ' + incidentParams);
        $.ajax({
            url: apiBaseUri + 'incidents/' + workingIncidentID,
            method: 'PUT',
            data: incidentParams
        })
        .done(function (data) {
            for (var key in localMods) postedMods[key] = localMods[key].val();
            localMods = {};
        })
        .fail(function (jqxhr, textStatus, error) {
            console.log(jqxhr);
            for (var key in localMods) localMods[key].css({ backgroundColor: 'LavenderBlush' });
        });
    }
}



function pullIncidentAndPopulate(incidentID) {
    $.getJSON(apiBaseUri + 'incidents/' + incidentID, function (data) {
        populateForm(data);
    })
    .fail(function (jqxhr, textStatus, error) {
        console.log(jqxhr);
        alert(error);
    });
}


function pullCommentsAndPopulate(incidentID) {
    $.getJSON(apiBaseUri + 'incidents/' + incidentID + '/comments', function (data) {
        var commentList = '';
        data.forEach(function (item, index, arr) {
            var commentator = item.UpdatedBy.split(':');
            commentList += '<p><strong>Comment Added by ' + (commentator[1] || commentator[0]) + ' - ' + formatDate(item.UpdatedOn, '') + '</strong></p> <p>' + item.Text + '</p>';
            //commentList += '<p><strong>Comment Added by ' + item.UpdatedBy + ' - ' + formatDate(item.UpdatedOn, '') + '</strong></p> <p>' + item.Text + '</p>';
        });
        $('#incidentHistory').html(commentList);
    })
    .fail(function (jqxhr, textStatus, error) {
        console.log(jqxhr);
    });
}


function pullStudentSearchResults(searchSchoolID, searchTerm) {
    resetAssignStudentModal(null, false);

    if ((searchTerm == null) || (searchTerm.trim().length == 0)) {
        $('#assignStudent-matchCountWarningNone').toggle(true);
        return;
    }

    $("#assignStudentSpinner").show();
    var param = $.param({ searchTerm: '%' + searchTerm + '%' });

    $.getJSON(apiBaseUri + 'schools/' + searchSchoolID + '/students', param, function (data) {
        var options = '';
        data.forEach(function (item, index, arr) {
            //options += '<option value="' + item.ID + '">(#' + item.StudentNumber + ')  (G ' + item.Grade + ') : ' + item.Name + '</option>';
            options += '<option value="' + item.ID + '" data-studentName="' + item.Name + '" data-studentNumber="' + item.StudentNumber +
                '">(#' + item.StudentNumber + ')  (G ' + item.Grade + ') : ' + item.Name + '</option>';
        });
        $("#assignStudentSpinner").hide();
        $('#assignStudent-matches').html(options);
        if (data.length < 1) $('#assignStudent-matchCountWarningNone').toggle(true);
        if (data.length > 99) $('#assignStudent-matchCountWarningExcessive').toggle(true);
    })
    .fail(function (jqxhr, textStatus, error) {
        console.log(jqxhr);
        $("#assignStudentSpinner").hide();
        $('#assignStudent-matchCountWarningNone').toggle(true);
    });
}


function pushIncidentUpdates(incidentID, incidentParams) {
    $.ajax({
        url: apiBaseUri + 'incidents/' + incidentID,
        method: 'PUT',
        data: incidentParams
    })
    .fail(function (jqxhr, textStatus, error) {
        console.log(jqxhr);
    });
}


function pushConcernedParty(incidentID, concernedPartyID) {
    var param = $.param({ 'ConcernedParty.ID': concernedPartyID });
    $.ajax({
        url: apiBaseUri + 'incidents/' + incidentID,
        method: 'PUT',
        data: param
    })
    .done(function (data) {
        var studentName = $('#assignStudent-matches').find("option:selected").text();
        $('#concernedParty').val(studentName);
        $('#assignStudentModal').modal("hide");
        resetAssignStudentModal(prevailingReportingSchoolID, true);
    })
    .fail(function (jqxhr, textStatus, error) {
        console.log(jqxhr);
    });
}


function pushComment(incidentID, commentText) {
    if ((commentText == null) || (commentText.trim().length == 0)) return;

    var param = $.param({ text: commentText });

    $.ajax({
        url: apiBaseUri + 'incidents/' + incidentID + '/comments',
        method: 'POST',
        data: param
    })
    .done(function (data) {
        $('#comment').val('');
        pullCommentsAndPopulate(incidentID);
    })
    .fail(function (jqxhr, textStatus, error) {
        console.log(jqxhr);
        alert(error);
    });
}



function populateForm(incident) {
    if (incident == null) return;

    var districtAdminYN = (userSchoolID == -1);
    var reportingSchoolID = 0; try { reportingSchoolID = incident.ReportingSchool.ID } catch (e) { }
    var correctedReportingSchoolID = 0; try { correctedReportingSchoolID = incident.CorrectedReportingSchool.ID } catch (e) { }
    var concernedPartyStudentNumber = 0; try { concernedPartyStudentNumber = incident.ConcernedParty.StudentNumber } catch (e) { }
    var reportingSchoolName = null; try { reportingSchoolName = incident.ReportingSchool.Name } catch (e) { }
    var correctedReportingSchoolName = null; try { correctedReportingSchoolName = incident.CorrectedReportingSchool.Name } catch (e) { }
    var reportingPartyName = null; try { reportingPartyName = incident.ReportingParty.Name } catch (e) { }
    var concernedPartyName = null; try { concernedPartyName = incident.ConcernedParty.Name } catch (e) { }
    var submitterName = null; try { submitterName = incident.Submitter.Name } catch (e) { }
    var formattedCorrectedIncidentDate = formatDate(incident.CorrectedIncidentDate, null);
    var formattedIncidentDate = formatDate(incident.IncidentDate, null);

    prevailingReportingSchoolID = (correctedReportingSchoolID > 0) ? correctedReportingSchoolID : reportingSchoolID;
    if (reportingSchoolID > 0) schoolMap[reportingSchoolID] = reportingSchoolName;
    if (correctedReportingSchoolID > 0) schoolMap[correctedReportingSchoolID] = correctedReportingSchoolName;

    $('#incidentID').text(incident.ID);
    $('#submitter').text(submitterName);
    $('#submittedOn').text(formatDate(incident.SubmittedOn, null));
    $('#description').text(incident.Description);
    $('#reportingSchool').text(reportingSchoolName);
    $('#incidentDate').text(formattedIncidentDate);
    $('#comment').text('');
    if (correctedReportingSchoolName) $('#correctedReportingSchool-readonly').text(correctedReportingSchoolName);  //otherwise leave info/warning (in markup file) as is

    $('#correctedIncidentDate').val(formattedCorrectedIncidentDate);
    $('#specificLocation').val(incident.SpecificLocation);
    $('#reportingParty').val(reportingPartyName);
    $('#concernedParty').val((incident.ConcernedParty == null) ? '' : '(#' + concernedPartyStudentNumber + ')  ' + concernedPartyName);

    $('#sourceCD').val(incident.SourceCD);
    $('#categoryCD').val(incident.CategoryCD);
    $('#statusCD').val(incident.StatusCD);
    $('#outcomeCD').val(incident.OutcomeCD);
    $('#correctedReportingSchoolID').val(correctedReportingSchoolID);

    $('#correctedReportingSchool-editable').toggle(districtAdminYN);
    $('#correctedReportingSchool-readonly').toggle(!districtAdminYN);

    if ((correctedReportingSchoolID > 0) || ((formattedCorrectedIncidentDate != null) && (formattedCorrectedIncidentDate != formattedIncidentDate))) {
        $('#correctionSwitch').toggle(false);
        $('#correctionPanel').toggle(true);
    } else {
        $('#correctionSwitch').toggle(true);
        $('#correctionPanel').toggle(false);
    }
    resetAssignStudentModal(prevailingReportingSchoolID, true);
    //$('#debug').text('reached the bottom of populateForm() \n' + correctedReportingSchoolID);
}


function resetAssignStudentModal(schoolSearchContext, resetTextFields) {
    $('#assignStudentCommand').prop('disabled', true);
    $('#assignStudent-matchCountWarningExcessive').toggle(false);
    $('#assignStudent-matchCountWarningNone').toggle(false);
    $('#assignStudent-matches').empty();
    $("#assignStudentSpinner").hide();
    if (resetTextFields) $('#assignStudent-searchSchool').text(schoolMap[schoolSearchContext]);
    if (resetTextFields) $('#assignStudent-searchTerm').val(null);
}


function formatDate(uDate, altText) {
    try {
        uDate = new Date(uDate);  //string -> Date conversion, if necessary. (fn will also accept argument as Date data type)
        if (uDate.getFullYear() < 1900) throw 'bad date';
        var t = uDate.toLocaleTimeString().split(':');
        var tAmPm = t[2].split(' ')[1];
        var fDate = uDate.toDateString().slice(4) + ', ' + [t[0], t[1]].join(':') + tAmPm.toLowerCase();
        return fDate;
    } catch (e) {
        return altText;
    }
}
