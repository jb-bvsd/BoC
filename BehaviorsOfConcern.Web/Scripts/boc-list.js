
//  note: these arrays are used by the script in this file, but they are declared/defined outside to
//        allow server-side to easily build their content  (refer to view @ List.cshtml)
//var incidentCategoryLookups = @Html.Raw(JsonConvert.SerializeObject(Model.IncidentCategoryLookups.ToDictionary(luc => luc.Value.ToString(), luc => luc.Label)));
//var sourceLookups = @Html.Raw(JsonConvert.SerializeObject(Model.SourceLookups.ToDictionary(luc => luc.Value.ToString(), luc => luc.Label)));
//var statusLookups = @Html.Raw(JsonConvert.SerializeObject(Model.StatusLookups.ToDictionary(luc => luc.Value.ToString(), luc => luc.Label)));
//var outcomeLookups = @Html.Raw(JsonConvert.SerializeObject(Model.OutcomeLookups.ToDictionary(luc => luc.Value.ToString(), luc => luc.Label)));
//var recencyIntervalLookups = @Html.Raw(JsonConvert.SerializeObject(Model.RecencyIntervalLookups.ToDictionary(luc => luc.Value.ToString(), luc => luc.Label)));
var timeoutId;
var apiBaseUri = sessionStorage.getItem('bocUri') || '';
var userName = sessionStorage.getItem('userName') || '';
var userSchoolID = Number(sessionStorage.getItem('userSchoolID') || '0');

//ready handler:
$(document).ready(function () {
    initializeForm(userName, userSchoolID);

    //ht tps://datatables.net/examples/ajax/orthogonal-data.html
    $.ajaxSetup({
        headers: {
            "Authorization": "Basic " + (sessionStorage.getItem('authToken') || '')
        }
    });

    //init DataTable.net
    var incidentListTable = $('#incidentList').DataTable({
        "info": true,
        "language": {
            "infoFiltered": "",
            "infoPostFix": "&nbsp;<em><span id='incidentList-refreshed'>(updated 2:34:56)</span></em>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;( * = corrected date/school)"
        },
        "paging": true,
        "ordering": true,
        "serverSide": true,
        "select": { style: "single" },
        "ajax": {
            "url": apiBaseUri + "incidents/search",
            "type": "POST",
            "data": function (d) {
                //d.Filters = $('#filterIncidentsForm').serialize();
                d.FilterStatus = $("[name=filterIncidents-Status]").serializeArray();
                d.FilterCategory = $("[name=filterIncidents-Category]").serializeArray();
                d.FilterOutcome = $("[name=filterIncidents-Outcome]").serializeArray();
                d.FilterRecency = $("input[name=filterIncidents-IncidentRecency]:checked", "#filterIncidentsForm").val()
                d.FilterSchool = $('#filterIncidents-reportingSchoolID').val();
            }
        },
        "columns": [
            { data: "ID", name: "ID", render: function (data, type, full, meta) {
                return '<a href="#!" onclick="openIncident(' + data + ');" >' + data + '</a>';
                //return '<button type="button" class="btn btn-info btn-sm">Info</button>';
            }},
            { data: "IncidentDate", name: "IncidentDateSort", render: function (data, type, full, meta) {
                var cid = new Date(full.CorrectedIncidentDate);
                if (cid.getFullYear() < 1900)
                    return data.slice(0, 10);
                else
                    return full.CorrectedIncidentDate.slice(0, 10) + '*';
            }},
            { data: "ConcernedParty.Name", name: "ConcernedParty", defaultContent: "" },
            { data: "", name: "ReportingSchoolSort", render: function (data, type, full, meta) {
                var reportingSchoolName = null; try { reportingSchoolName = full.ReportingSchool.Name; } catch (e) { }
                var correctedReportingSchoolName = null; try { correctedReportingSchoolName = full.CorrectedReportingSchool.Name; } catch (e) { }
                //return correctedReportingSchoolName || reportingSchoolName;
                if (correctedReportingSchoolName) return correctedReportingSchoolName + '*'; else return reportingSchoolName;
            }},
            { data: "CategoryCD", name: "CategorySort", render: function (data, type, full, meta) { return incidentCategoryLookups[data] || ""; } },
            { data: "SourceCD", name: "SourceSort", render: function (data, type, full, meta) { return sourceLookups[data] || ""; } },
            { data: "StatusCD", name: "StatusSort", render: function (data, type, full, meta) { return statusLookups[data] || ""; } },
            { data: "SubmittedOn", name: "SubmittedOn", render: function (data, type, full, meta) { return data.slice(0, 10); } }
        ]
    });


    incidentListTable
        .on('select', function (e, dt, type, indexes) {
            var rowData = incidentListTable.rows(indexes).data().toArray();
            //console.log(rowData[0]);
            showIncidentReport(rowData[0]);
        })
        .on('deselect', function (e, dt, type, indexes) {
            hideIncidentReport();
        });


    //add custom help text to DataTable.net
    $('#incidentList_filter').prepend("<em>(Note: Shft + click to sort mult. columns)</em>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");


    //handle list redraw event
    $('#incidentList').on('draw.dt', function () {
        //console.log('Redraw occurred at: ' + new Date().getTime());
        var time = (new Date()).toLocaleTimeString();
        $('#incidentList-refreshed').text('(refreshed @ ' + time + ')');
    });


    //handle open incident command
    $('#openIncidentCommand').on('click', function (event) {
        window.location.assign($(this).attr('data-redirectUrl'));
    });


    //handle modal create incident command
    $('#createIncidentCommand').on('click', function (event) {
        //var redirectUrl = $(this).data('redirectUrl');
        //var redirectUrl = $(this).attr('data-redirectUrl');
        var formdata = $('#newIncidentForm')
            .serialize()
            .replace(/newIncident-reportingSchoolID=/gi, "reportingSchool.ID=")
            .replace(/newIncident-/gi, "");

        //pushIncident(formdata, redirectUrl);
        pushIncident(formdata);
    });

    //handle modal hidden event
    $('#newIncidentModal').on('hidden.bs.modal', function (e) {
        resetNewIncidentModal();
    });

    //handle filter's refresh list command
    $('#refreshIncidentListCommand').on('click', function (event) {
        $('#incidentList').DataTable().ajax.reload();
    });


    //handle filter choice changes (timed event)
    $('.filterOption').on('click', resetFilterTimer);
    $('#filterIncidents-reportingSchoolID').on('change', resetFilterTimer);
});
//end - ready handler





function initializeForm(userName, userSchoolID) {
    sessionStorage.removeItem('workingIncidentID');

    hideIncidentReport();
    $('#filterPanelIncidents-Category').collapse("hide");  //explicit call triggers display of proper glyph (should be handled by lack of "collapse in" in FilterGroup helper, but isn't)
    $('#filterPanelIncidents-Outcome').collapse("hide");  //explicit call triggers display of proper glyph (should be handled by lack of "collapse in" in FilterGroup helper, but isn't)
    $('#sessionUserName').text(userName || "");

    if (userSchoolID == -1) {
        $('#filterIncidents-reportingSchoolID').prop('disabled', false);
        $('#newIncident-reportingSchoolID').prop('disabled', false);
    } else {
        $('#filterIncidents-reportingSchoolID').val(userSchoolID);
        $('#filterIncidents-reportingSchoolID').prop('disabled', true);
        $('#newIncident-reportingSchoolID').val(userSchoolID);
        $('#newIncident-reportingSchoolID').prop('disabled', true);
    }

    resetNewIncidentModal();
}


function resetNewIncidentModal() {
    $("#newIncidentSpinner").hide();
    $('#newIncident-validationWarningText').html("");
    $('#newIncident-validationWarning').toggle(false);
}


function resetFilterTimer() {
    if (timeoutId) clearTimeout(timeoutId);
    timeoutId = setTimeout(function () { $('#incidentList').DataTable().ajax.reload(); }, 50);
}


function pushIncident(incidentParams) {
    $("#newIncidentSpinner").show();
    $.ajax({
        url: apiBaseUri + 'incidents',
        method: 'POST',
        data: incidentParams
    })
    .done(function (data) {
        resetNewIncidentModal();
        openIncident(data);
    })
    .fail(function (jqxhr, textStatus, error) {
        $("#newIncidentSpinner").hide();
        $('#newIncident-validationWarningText').html(jqxhr.responseJSON.Message.replace(/\n/g, "<br/>"));
        $('#newIncident-validationWarning').toggle(true);
    });
}


function openIncident(incidentID) {
    sessionStorage.setItem('workingIncidentID', incidentID.toString());
    window.location.assign($('#openIncidentCommand').attr('data-redirectUrl'));
}


function hideIncidentReport() {
    $('#incidentReportForm').toggle(false);
    $('#openIncidentCommand').prop('disabled', true);
    //sessionStorage.removeItem('workingIncidentID');  //don't remove ID from session since "border-line" user clicks may both trigger an anchor tag navigation AND fire the 'deselect' event on incidentListTable, leaving the Details page with a bad ID value
}


function showIncidentReport(incident) {
    var formattedCorrectedIncidentDate = formatDate(incident.CorrectedIncidentDate, null);
    var formattedIncidentDate = formatDate(incident.IncidentDate, null);
    var correctedReportingSchoolName = null; try { correctedReportingSchoolName = incident.CorrectedReportingSchool.Name; } catch (e) { }

    sessionStorage.setItem('workingIncidentID', incident.ID.toString());

    $('#incidentID').text(incident.ID);
    $('#submitter').text(incident.Submitter.Name);
    $('#submittedOn').text(formatDate(incident.SubmittedOn));
    $('#description').text(incident.Description);
    $('#reportingSchool').text((correctedReportingSchoolName) ? correctedReportingSchoolName + '*' : incident.ReportingSchool.Name);
    $('#incidentDate').text((formattedCorrectedIncidentDate) ? formattedCorrectedIncidentDate + '*' : formattedIncidentDate);

    $('#incidentReportForm').toggle(true);
    $('#openIncidentCommand').prop('disabled', false);
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
