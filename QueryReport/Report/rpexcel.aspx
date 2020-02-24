<%@ Page Title="" Language="C#" MasterPageFile="~/Controls/mp.Master" AutoEventWireup="true" CodeBehind="rpexcel.aspx.cs" Inherits="QueryReport.rpexcel" EnableEventValidation="false" %>
<asp:Content ID="header" ContentPlaceHolderID="header" runat="server">
    <link rel="stylesheet" type="text/css" href="../assets/stylesheets/fa4/css/font-awesome.min.css" />
    <style type="text/css">
        .LabelStyle
        {
            margin-right: 5px;
        }
        .LabelStyle > SPAN
        {
            font-weight: normal;
            font-style: italic;
        }
        .SelectionBlock
        {
            width: 210px;
            height: 30px;
            border: 1px solid black;
            margin: 2px;
            padding: 2px;
        }
        /*.form-control.n_lines_3
        {
            height: 72px !important;
            margin-left: -4px;
        }*/
        #txtSQLText
        {
            font-family: monospace;
            font-size: 16px;
        }

        .grouped {
	        color: gray
        }

        .asc:after {
           content: "【ASCENDING】";
           color: darkgreen
        }

        .desc:after {
           content: "【DESCENDING】";
           color: indigo
        }
    </style>
    <script type="text/javascript">
        //var txtReportNameChangedrunning = false;
        var gflag = false;
        function myalert(m, o) {
            if (gflag) {
                return;
            }

            gflag = true;
            alert(m);
            //o.focus();
            o.select();
            setTimeout(function () { gflag = false; }, 100);
        }

        function txtReportNameChanged() {
            /*
            if (txtReportNameChangedrunning) return;
            txtReportNameChangedrunning = true;
            var ReportName = $('[data-controlname=txtReportName]').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=checkreportname",
                type: "POST",
                data: {
                    ReportName: ReportName
                },
                success: function (data, textStatus, jqXHR) {
                    txtReportNameChangedrunning = false;
                    if (data.indexOf('OK') >= 0) {
                        if (!$('[data-controlname=txtReportTitle]').val()) {
                            $('[data-controlname=txtReportTitle]').val(ReportName);
                        };
                    } else if (data.indexOf('DOCTYPE') >= 0) {
                        alert("Session expired. Please login again.");
                        $('[data-controlname=txtReportName]').select();
                    } else {
                        alert(data);
                        $('[data-controlname=txtReportName]').select();
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    txtReportNameChangedrunning = false;
                    alert(errorThrown);
                }
            });
            return false;
            */
            "use strict";
            //console.log("a");
            return function () {
                var ReportName = $('[data-controlname=txtReportName]').val();
                jQuery.ajax({
                    url: "rpexcel.aspx?action=checkreportname",
                    type: "POST",
                    data: {
                        ReportName: ReportName
                    },
                    success: function (data, textStatus, jqXHR) {
                        //txtReportNameChangedrunning = false;
                        if (data.indexOf('OK') >= 0) {
                            if (!$('[data-controlname=txtReportTitle]').val()) {
                                $('[data-controlname=txtReportTitle]').val(ReportName);
                            };
                        } else if (data.indexOf('DOCTYPE') >= 0) {
                            //alert("Session expired. Please login again.");
                            //$('[data-controlname=txtReportName]').select();
                            myalert("Session expired. Please login again.", $('[data-controlname=txtReportName]'));
                        } else {
                            //alert(data);
                            //$('[data-controlname=txtReportName]').select();
                            myalert(data, $('[data-controlname=txtReportName]'));
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        //txtReportNameChangedrunning = false;
                        alert(errorThrown);
                    }
                });
                //return false;
            }
        }

        function txtAllColumnsFilter_invoked(e) {
            var target = $("#txtAllColumnsFilter").val().toUpperCase();
            window.status = target;
            $("[data-controlname=lbAllColumns] option").remove();
            $("[data-controlname=lbAllColumns_master] option").each(function (i, selected) {
                var opt = $(selected);
                if (opt.text().toUpperCase().indexOf(target) >= 0) {
                    $("[data-controlname=lbAllColumns]").append(opt.clone());
                }
            });
            return false;
        }

        function Validate_Action(ctrlname, action, selection) {
            switch (action) {
                case "add":
                    {
                        switch (ctrlname) {
                            case "lbhiden":
                            case "lbsorton":
                            case "lbsum":
                            case "lbavg":
                            case "lbrpcount":
                            case "lbgroupavg":
                            case "lbgrouptotal":
                            case "lbgroupcount":
                                {
                                    if (selection == null) return false;
                                    var elements = $('[data-controlname=lbcontents] option');
                                    var i;
                                    var j;
                                    var found;
                                    for (i = 0; i < selection.length; i++) {
                                        found = false;
                                        j = 0;
                                        while ((!found) && (j < elements.length)) {
                                            if (elements[j].value == selection[i].value) {
                                                found = true;
                                            }
                                            j++;
                                        }
                                        if (!found) {
                                            alert("Field [" + selection[i].text + "] must be added to content first!");
                                            return false;
                                        }
                                    }
                                    return true;
                                }
                                break;
                            case "":    // place holder
                                {
                                    if (selection == null) return false;
                                    var elements = $('[data-controlname=lbhiden] option');
                                    var i;
                                    var j;
                                    var found;
                                    for (i = 0; i < selection.length; i++) {
                                        found = false;
                                        j = 0;
                                        while ((!found) && (j < elements.length)) {
                                            if (elements[j].value == selection[i].value) {
                                                found = true;
                                            }
                                            j++;
                                        }
                                        if (!found) {
                                            alert("Field [" + selection[i].text + "] must be added to group first!");
                                            return false;
                                        }
                                    }
                                    return true;
                                }
                                break;
                        }
                        return true;
                    }
                    break;
                case "remove":
                    {
                        switch (ctrlname) {
                            case "lbcontents":
                                {
                                    var selection = $('[data-controlname=lbcontents] option:selected');
                                    var ctrllist = ['lbhiden', 'lbsorton', 'lbsum', 'lbavg', 'lbrpcount', 'lbgroupavg', 'lbgroupcount', 'lbgrouptotal'];
                                    var ctrlcnt = 0;
                                    var elements = null;
                                    var i;
                                    var j;
                                    var found;

                                    for (ctrlcnt = 0; ctrlcnt < ctrllist.length; ctrlcnt++) {
                                        elements = $('[data-controlname=' + ctrllist[ctrlcnt] + '] option');

                                        for (i = 0; i < selection.length; i++) {
                                            found = false;
                                            j = 0;
                                            while ((!found) && (j < elements.length)) {
                                                if (elements[j].value == selection[i].value) {
                                                    found = true;
                                                }
                                                j++;
                                            }
                                            if (found) {
                                                if (ctrllist[ctrlcnt] == 'lbhiden') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group fields first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbsorton') {
                                                    alert("Field [" + selection[i].text + "] must be removed from sort fields first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbsum') {
                                                    alert("Field [" + selection[i].text + "] must be removed from report total first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbavg') {
                                                    alert("Field [" + selection[i].text + "] must be removed from report avg first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbrpcount') {
                                                    alert("Field [" + selection[i].text + "] must be removed from report count first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgroupavg') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group average first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgroupcount') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group count first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgrouptotal') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group total first!");
                                                }
                                                return false;
                                            }
                                        }
                                    }
                                    return true;
                                }
                                break;
                            case "lbhiden":
                                {
                                    var selection = $('[data-controlname=lbhiden] option:selected');
                                    var ctrllist = ['lbgroupavg', 'lbgroupcount', 'lbgrouptotal'];
                                    var ctrlcnt = 0;
                                    var elements = null;
                                    var i;
                                    var j;
                                    var found;

                                    for (ctrlcnt = 0; ctrlcnt < ctrllist.length; ctrlcnt++) {
                                        elements = $('[data-controlname=' + ctrllist[ctrlcnt] + '] option');

                                        for (i = 0; i < selection.length; i++) {
                                            found = false;
                                            j = 0;
                                            while ((!found) && (j < elements.length)) {
                                                if (elements[j].value == selection[i].value) {
                                                    found = true;
                                                }
                                                j++;
                                            }
                                            if (found) {
                                                if (ctrllist[ctrlcnt] == 'lbgroupavg') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group average first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgroupcount') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group count first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgrouptotal') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group total first!");
                                                }
                                                return false;
                                            }
                                        }
                                    }
                                    return true;
                                }
                                break;
                        }
                        return true;
                    }
                    break;
                case "reset":
                    {
                        switch (ctrlname) {
                            case "lbhiden":
                                {
                                    var selection = $('[data-controlname=lbhiden] option');
                                    var ctrllist = ['lbgroupavg', 'lbgroupcount', 'lbgrouptotal'];
                                    var ctrlcnt = 0;
                                    var elements = null;
                                    var i;
                                    var j;
                                    var found;

                                    for (ctrlcnt = 0; ctrlcnt < ctrllist.length; ctrlcnt++) {
                                        elements = $('[data-controlname=' + ctrllist[ctrlcnt] + '] option');

                                        for (i = 0; i < selection.length; i++) {
                                            found = false;
                                            j = 0;
                                            while ((!found) && (j < elements.length)) {
                                                if (elements[j].value == selection[i].value) {
                                                    found = true;
                                                }
                                                j++;
                                            }
                                            if (found) {
                                                if (ctrllist[ctrlcnt] == 'lbgroupavg') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group average first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgroupcount') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group count first!");
                                                } else if (ctrllist[ctrlcnt] == 'lbgrouptotal') {
                                                    alert("Field [" + selection[i].text + "] must be removed from group total first!");
                                                }
                                                return false;
                                            }
                                        }
                                    }
                                    return true;
                                }
                                break;
                        }
                        return true;
                    }
                    break;
            }
            alert("Validation rule is undefined");
            return false;
        }

        function btnAdd_Clicked(src, ctrlname, d) {
            var selection = [];
            if ((src == "lbAllColumns") || (src == "lbFormula")) {
                $('[data-controlname=' + src + '] :selected').each(function (i, selected) {
                    selection[i] = selected;
                });
            } else {
                $('[data-controlname=' + src + '] option').each(function (i, selected) {
                    selection[i] = selected;
                });
            }

            if (d == null) { // d indicates it's triggered action so bypass validation to prevent going into loop.
                if (!Validate_Action(ctrlname, "add", selection)) {
                    console.log('Cannot be added');
                    return false;
                }
            }

            var i = 0;
            var j = 0;
            var currentitem = null;
            var found = false;
            var optionstate = false;
            if (d != null) { optionstate = d; }
            var ctrl = $('[data-controlname=' + ctrlname + ']');
            var elements = $('[data-controlname=' + ctrlname + '] option');
            var greylength = 0;

            if ((ctrlname == "lbsorton") && (elements.length > 0)) {
                greylength = $('[data-controlname=' + ctrlname + '] option').filter('.grouped').length;
            }

            var optlist = ctrl[0].options;
            for (i = 0; i < selection.length; i++) {
                currentitem = $(selection[i]);
                // check for duplicates
                found = false;
                for (j = 0; j < optlist.length; j++) {
                    if (optlist[j].value == currentitem.val()) {
                        //if (d != null) {
                        //    $(optlist[j]).prop("disabled", d);    // touch the "disabled" state
                        //}
                        if (d) {
                            $(optlist[j]).addClass('grouped');
                        }
                        found = true;
                    }
                }
                if (!found) {
                    if ((!d) || (elements.length < 1)) {
                        var newEle = $("<option></option>").attr("value", currentitem.val()).text(currentitem.text())
                        .prop("data-datatype", currentitem.prop("data-datatype"));
                        if (optionstate) newEle.addClass('grouped');
                        if (ctrlname == 'lbsorton') {
                            newEle.addClass('asc');
                        }
                        ctrl.append(newEle);
                        if (d) { greylength++; }
                    } else {
                        if (greylength < 1) {
                            var newEle = $("<option></option>").attr("value", currentitem.val()).text(currentitem.text())
                        .prop("data-datatype", currentitem.prop("data-datatype"));
                            if (optionstate) newEle.addClass('grouped');
                            if (ctrlname == 'lbsorton') {
                                newEle.addClass('asc');
                            }
                            $('[data-controlname=' + ctrlname + '] option').eq(0).before(newEle);
                            greylength++;
                        } else {
                            var newEle = $("<option></option>").attr("value", currentitem.val()).text(currentitem.text())
                            .prop("data-datatype", currentitem.prop("data-datatype"));
                            if (optionstate) newEle.addClass('grouped');
                            if (ctrlname == 'lbsorton') {
                                newEle.addClass('asc');
                            }
                            $('[data-controlname=' + ctrlname + '] option').filter('.grouped').last().after(newEle);
                        }
                    }
                }
            }

            // lbhiden
            if (ctrlname == 'lbhiden') {
                btnRemove_Clicked('lbsorton', true);
                btnAdd_Clicked('lbhiden', 'lbsorton', true);
            }

            return false;
        }

        function btnRemove_Clicked(ctrlname, d) {
            if (d == null) { // d indicates it's triggered action so bypass validation to prevent going into loop.
                if (!Validate_Action(ctrlname, "remove")) return false;
            }

            //if (d) {
            //    $('[data-controlname=' + ctrlname + '] :disabled').each(function (i, selected) {
            //        $(selected).remove();
            //    });
            //} else {
            //    $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
            //        $(selected).remove();
            //    });
            //}

            if ($('[data-controlname=lbsorton] :selected').filter('.grouped').length != 0) return false;

            if (d) {
                $('[data-controlname=' + ctrlname + '] option').filter('.grouped').each(function (i, selected) {
                    $(selected).remove();
                });
            } else {
                $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                    $(selected).remove();
                });
            }

            // lbhiden
            if (ctrlname == 'lbhiden') {
                btnRemove_Clicked('lbsorton', true);
                btnAdd_Clicked('lbhiden', 'lbsorton', true);
            }

            return false;
        }

        function btnReset_Clicked(ctrlname) {
            if (!Validate_Action(ctrlname, "reset")) return false;

            $('[data-controlname=' + ctrlname + '] option').each(function (i, selected) {
                $(selected).remove();
            });

            if (ctrlname == 'lbhiden') {
                btnRemove_Clicked('lbsorton', true);
            }
            return false;
        }

        function btnUp_Clicked(ctrlname) {
            if ($('[data-controlname=' + ctrlname + '] :selected').filter('.grouped').length != 0) return false;
            $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                var opt_prev = $(selected).prev();
                if (!opt_prev.hasClass('grouped')) {
                    $(selected).insertBefore(opt_prev);
                }
            });
            return false;
        }

        function btnDown_Clicked(ctrlname) {
            if ($('[data-controlname=' + ctrlname + '] :selected').filter('.grouped').length != 0) return false;
            $($('[data-controlname=' + ctrlname + '] :selected').get().reverse()).each(function (i, selected) {
                $(selected).insertAfter($(selected).next());
            });
            return false;
        }

        function btnGroupby_Up_Clicked() {
            $('[data-controlname="lbhiden"] :selected').each(function (i, selected) {
                var index = $(selected).index();
                var sorton_selected = $('[data-controlname="lbsorton"] option:eq(' + index + ')');
                var sorton_opt_prev = sorton_selected.prev();
                sorton_selected.insertBefore(sorton_opt_prev);

                var opt_prev = $(selected).prev();
                if (!opt_prev.hasClass('grouped')) {
                    $(selected).insertBefore(opt_prev);
                }
            });
            return false;
        }

        function btnGroupby_Down_Clicked() {
            $($('[data-controlname="lbhiden"] :selected').get().reverse()).each(function (i, selected) {
                var next = $(selected).next();
                if (next.length == 0) return false;

                var index = $(selected).index();
                var sorton_selected = $('[data-controlname="lbsorton"] option:eq(' + index + ')');
                var sorton_opt_next = sorton_selected.next();
                sorton_selected.insertAfter(sorton_opt_next);

                $(selected).insertAfter(next);
            });
            return false;
        }

        function btnSortonAsc_Clicked() {
            $('[data-controlname="lbsorton"] :selected').each(function (i, selected) {
                $(selected).removeClass("desc");
                $(selected).addClass("asc");
            });
            return false;
        }

        function btnSortonDesc_Clicked() {
            $('[data-controlname="lbsorton"] :selected').each(function (i, selected) {
                $(selected).removeClass("asc");
                $(selected).addClass("desc");
            });
            return false;
        }

        function unselectAll(ctrlname) {
            if (ctrlname == null) {
                $('SELECT[multiple] :selected').each(function (i, selected) {
                    $(selected).prop("selected", false);
                });
            } else {
                $('[data-controlname=' + ctrlname + '] :selected').each(function (i, selected) {
                    $(selected).prop("selected", false);
                });
            }
            return true;
        }

        function lbFormula_selectionChanged() {
            var formulas = $('[data-controlname=lbFormula] option');
            var cnt = 0;
            var i = 0;
            for (i = 0; i < formulas.length; i++) {
                if (formulas[i].selected) {
                    $('#hid_formulaidx').val(i);
                    cnt++;
                }
            }
            // disable edit button if not exactly 1 button is pressed
            if (cnt != 1) {
                $('#btnFormulaEdit').addClass("disabled");
                $('#btnFormulaDelete').addClass("disabled");
            } else {
                $('#btnFormulaEdit').removeClass("disabled");
                $('#btnFormulaDelete').removeClass("disabled");
            }

            if ($('[data-controlname=lbFormula] option:selected[numeric="false"]').size() > 0) {
                console.log('Disabled!');
                //Override onclick property
                $('[data-controlname=btnSumInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbsum')");
                $('[data-controlname=btnAvgInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbavg')");
                $('[data-controlname=btnRpcountInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbrpcount')");

                $('[data-controlname=btnGrouptotalInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgrouptotal')");
                $('[data-controlname=btnGroupavgInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupavg')");
                $('[data-controlname=btnGroupcountInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupcount')");
            } else {
                console.log('Enabled!');
                $('[data-controlname=btnSumInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbsum') || btnAdd_Clicked('lbFormula', 'lbsum')");;
                $('[data-controlname=btnAvgInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbavg') || btnAdd_Clicked('lbFormula', 'lbavg')");
                $('[data-controlname=btnRpcountInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbrpcount') || btnAdd_Clicked('lbFormula', 'lbrpcount')");

                $('[data-controlname=btnGrouptotalInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgrouptotal') || btnAdd_Clicked('lbFormula', 'lbgrouptotal')");
                $('[data-controlname=btnGroupavgInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupavg') || btnAdd_Clicked('lbFormula', 'lbgroupavg')");
                $('[data-controlname=btnGroupcountInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupcount') || btnAdd_Clicked('lbFormula', 'lbgroupcount')");
            }
        }

        function newFormula() {
            // set index to the next item
            var idx = $('[data-controlname=lbFormula] option').length;
            $('#hid_formulaidx').val(idx);
            return true;
        }

        function removeFormula() {
            if ($('[data-controlname=lbFormula] option:selected').length != 1) {
                alert("You can only remove one item at a time");
                return false;
            }
            var idx = -1;
            var i = 0;
            var opts = $('[data-controlname=lbFormula] option');
            for (i = 0; i < opts.length; i++) {
                if (opts[i].selected) {
                    idx = i;
                }
            }
            jQuery.ajax({
                url: "rpexcel.aspx?action=removeFormula",
                type: "POST",
                data: {
                    id: idx
                },
                success: function (data, textStatus, jqXHR) {
                    $('option[value="' + data.DeletedFormulaName + '"]').remove();
                    if (data.fields != null) {
                        $('[data-controlname=lbFormula] option').remove();
                        var ctrl = $('[data-controlname=lbFormula]');
                        var i = 0
                        for (i = 0; i < data.fields.length; i++) {
                            ctrl.append($("<option></option>").attr("value", data.fields[i]).text(data.fields[i]));
                        }

                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }

        function pageSubmission() {
            var payload = {};
            var optlist = null;
            var i = 0;

            payload.ReportName = $("[data-controlname=txtReportName]").val();
            payload.ReportTitle = $("[data-controlname=txtReportTitle]").val();
            payload.SVID = $("[data-controlname=ddlQueryName]").val();
            payload.ReportGroupID = $("[data-controlname=ddlReportGroup]").val();
            payload.CategoryID = $("[data-controlname=ddlCategory]").val();
            payload.ReportType = ($("[data-controlname=ddlShowType]").val());
            payload.fHideCriteria = $("[data-controlname=chkHideCriteria] INPUT[type=checkbox]")[0].checked;
            payload.fHideDuplicate = $("[data-controlname=chkHideDuplicate] INPUT[type=checkbox]")[0].checked;
            payload.Format = $("[data-controlname=ddlFormat]").val();
            //payload.ReportHeader = $("[data-controlname=txtReportHeader]").val();
            //payload.ReportFooter = $("[data-controlname=txtReportFooter]").val();

            payload.contentColumn = [];
            optlist = $('[data-controlname=lbcontents] option');
            for (i = 0; i < optlist.length; i++) {
                payload.contentColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.criteriaColumn = [];
            optlist = $('[data-controlname=lbcriteria] option');
            for (i = 0; i < optlist.length; i++) {
                payload.criteriaColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.sortonColumn = [];
            optlist = $('[data-controlname=lbsorton] option');
            var is_asc = $('[data-controlname=lbsorton] option').map(function () { return $(this).hasClass('asc') });
            for (i = 0; i < optlist.length; i++) {
                payload.sortonColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                    // v1.8.8 Alex 2018.10.28 - Add IsAscending field 
                   , IsAscending: is_asc[i]
                });
            }

            payload.groupColumn = [];
            optlist = $('[data-controlname=lbhiden] option');
            for (i = 0; i < optlist.length; i++) {
                payload.groupColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.sumColumn = [];
            optlist = $('[data-controlname=lbsum] option');
            for (i = 0; i < optlist.length; i++) {
                payload.sumColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.grouptotalColumn = [];
            optlist = $('[data-controlname=lbgrouptotal] option');
            for (i = 0; i < optlist.length; i++) {
                payload.grouptotalColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.avgColumn = [];
            optlist = $('[data-controlname=lbavg] option');
            for (i = 0; i < optlist.length; i++) {
                payload.avgColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.groupavgColumn = [];
            optlist = $('[data-controlname=lbgroupavg] option');
            for (i = 0; i < optlist.length; i++) {
                payload.groupavgColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.rpcountColumn = [];
            optlist = $('[data-controlname=lbrpcount] option');
            for (i = 0; i < optlist.length; i++) {
                payload.rpcountColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            payload.groupcountColumn = [];
            optlist = $('[data-controlname=lbgroupcount] option');
            for (i = 0; i < optlist.length; i++) {
                payload.groupcountColumn.push({
                    ColumnName: optlist[i].value,
                    DisplayName: optlist[i].text
                });
            }

            jQuery.ajax({
                url: "rpexcel.aspx?action=pageSubmission",
                type: "POST",
                data: { payload: JSON.stringify(payload) },
                async: false,
                success: function (data, textStatus, jqXHR) {
                    if (data == "OK") {
                        unselectAll();
                        $('[data-controlname=btnNext]').click();
                    } else if (data.indexOf('DOCTYPE') > 0) {
                        alert("Session expired. Please login again.");
                    } else if (data.indexOf('in use') > 0) {
                        // do nothing as user will be prompt by txtReportNameChanged check
                    } else {
                        alert(data);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    //only error handler is needed
                    alert(errorThrown);
                }
            });
            return false;
        }

        function copyRpt() {
            jQuery.ajax({
                url: "rpexcel.aspx?action=copy",
                type: "POST",
                data: {
                    id: decodeURIComponent(window.location.search.replace(new RegExp("^(?:.*[&\\?]" + encodeURIComponent("id").replace(/[\.\+\*]/g, "\\$&") + "(?:\\=([^&]*))?)?.*$", "i"), "$1"))
                },
                success: function (data, textStatus, jqXHR) {
                    if (data == '_') {
                        alert('Report name too long. Please set a shorter report name before copying it.')
                    } else {
                        location.replace(data)
                    }
                }
            });
            return false;
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class='page-header'>
        <h1 class='pull-left'>
            <span>Report Detail - Report Infomation</span><input type="button" id="btnNext" runat="server" onserverclick="btnNext_Click" data-controlname="btnNext" style="display: none" />
        </h1>
        <div class='pull-right'>
            <div class='btn-group'>
                <a id="btnSubmit" class="btn btn-primary" onclick="return pageSubmission();"><i class="fa fa-arrow-right fa-lg"></i> Next</a>
            </div>
            <div class='btn-group'>
                 <a id="btnShowDeleteConfirm" class="btn btn-primary" data-toggle="modal" data-target="#delconfirm"><i class="fa fa-trash fa-lg"></i> Delete</a>
            </div>
            <div class='btn-group'>
                <a id="btnRefresh" class="btn btn-primary" runat="server" onserverclick="btnRefresh_Click"><i class="fa fa-refresh fa-lg"></i> Refresh</a>
            </div>
            <div class='btn-group'>
                <a id="btnCopy" class="btn btn-primary" runat="server" onclick="return copyRpt();"><i class="fa fa-files-o fa-lg"></i> Copy</a>
            </div>
        </div>
    </div>
    <div class="error" style="color: Red;">
        <asp:Literal ID="lblErrorText" runat="server"></asp:Literal></div>
    <div class='row'>
        <div class='col-xs-12'>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Report Name</label></div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportName" runat="server" CssClass="form-control" data-controlname="txtReportName" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox>
                        <label class="label">
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtReportName" ErrorMessage="*" ValidationGroup="sub" ForeColor="Red"></asp:RequiredFieldValidator></label>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Report Title</label></div>
                    <div class="col-xs-8">
                        <asp:TextBox ID="txtReportTitle" runat="server" CssClass="char-max-length form-control inline" data-controlname="txtReportTitle" MaxLength="50" Style='margin-bottom: 0;'></asp:TextBox></div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Query Name</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlQueryName" runat="server" AutoPostBack="true" data-controlname="ddlQueryName" OnSelectedIndexChanged="ddlQueryName_SelectedIndexChanged" CssClass="form-control inline">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ddlQueryNameRequiredFieldValidator" runat="server" ErrorMessage="*" ControlToValidate="ddlQueryName" ForeColor="Red" ValidationGroup="SAVE"></asp:RequiredFieldValidator>
                     </div>
                </div>
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Hide Criteria</label>
                    </div>
                    <div class="col-xs-2">
                        <asp:CheckBox ID="chkHideCriteria" runat="server" data-controlname="chkHideCriteria" Checked="true" />
                    </div>
                    <div class="col-xs-4">
                        <label class="LabelStyle">Hide Duplicate</label>
                    </div>
                    <div class="col-xs-2">
                        <asp:CheckBox ID="chkHideDuplicate" runat="server" data-controlname="chkHideDuplicate" Checked="true" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Report Group</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlReportGroup" data-controlname="ddlReportGroup" runat="server" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Category</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlCategory" data-controlname="ddlCategory" runat="server" CssClass="form-control inline">
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label>Report Type</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlShowType" data-controlname="ddlShowType" runat="server" CssClass="form-control inline">
                            <asp:ListItem Text="Show All" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Show changed data only" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Data Export" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Pivotable Template" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="col-xs-4">
                        <label class="LabelStyle">Default Format</label></div>
                    <div class="col-xs-8">
                        <asp:DropDownList ID="ddlFormat" data-controlname="ddlFormat" runat="server" CssClass="form-control inline">
                            <asp:ListItem Text="Excel" Value="0"></asp:ListItem>
                            <asp:ListItem Text="On Screen" Value="1"></asp:ListItem>
                            <asp:ListItem Text="PDF" Value="2"></asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
            </div>
            <br />
            <div class="row">
                <div class="col-xs-6">
                    <div class="col-xs-4"><label>Pivotable Template</label></div>
                    <div class="col-xs-8"><input type="button" id="btnTemplateManagement" value="Template Management" class="btn form-control" data-toggle="modal" data-target="#templatemgt" /></div>
                </div>
            </div>
        </div>
    </div>
    <div style="height: 20px">
        &nbsp;</div>
    <div class="row">
        <div class='col-md-4' style="min-width: 210px; max-width: 280px; padding-left: 5px;">
            <div class='faq'>
                <div class='tabbable'>
                    <ul class='nav nav-responsive nav-tabs'>
                        <li class='active'><a href="#tabAllColumns" data-toggle="tab">Columns</a></li>
                        <li><a href="#tabFormulas" data-toggle="tab">Fomulas</a></li>
                    </ul>
                    <div class='tab-content' style="margin: 0px; padding: 0px; height: 255px">
                        <div class='tab-pane active' id="tabAllColumns" style="height: 255px">
                            <div class='panel panel-default' style="height: 255px">
                                <div class='panel-heading' style="margin: 1px; padding: 1px; height: 255px">
                                    <label for="txtAllColumnsFilter" style="height: 30px; width: 30%; float: left; margin-top: 5px; margin-left: 5px">Search:</label><input type="text" id="txtAllColumnsFilter" class="form-control" style="width: 65%; float: right" autocomplete="off" placeholder="Search..." />
                                    <asp:ListBox ID="lbAllColumns_master" runat="server" SelectionMode="Multiple" CssClass="col-sm-12" Rows="30" style="display: none" data-controlname="lbAllColumns_master"></asp:ListBox>
                                    <asp:ListBox ID="lbAllColumns" runat="server" SelectionMode="Multiple" CssClass="col-sm-12" Rows="10" data-controlname="lbAllColumns" style="height: 200px"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                        <div class='tab-pane' id="tabFormulas" style="height: 255px">
                            <div class='panel panel-default' style="height: 255px">
                                <div class='panel-heading center-block text-center' style="margin: 1px; padding: 1px; height: 255px">
                                    <div class="btn-group center-block" style="margin-top: 15px; margin-bottom: 15px">
                                        <input type="button" id="btnFormulaNew" class="btn" value="New" data-toggle="modal" data-target="#dlgFormula" onclick="return newFormula();">
                                        <input type="button" id="btnFormulaEdit" class="btn disabled" value="Edit" data-toggle="modal" data-target="#dlgFormula" />
                                        <input type="button" id="btnFormulaDelete" class="btn disabled" value="Delete" onclick="return removeFormula();" />
                                    </div>
                                    <asp:ListBox ID="lbFormula" runat="server" SelectionMode="Multiple" CssClass="col-sm-12" Rows="26" data-controlname="lbFormula" style="height: 200px"></asp:ListBox>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-md-8" style="padding-left: 5px; margin-right: 0px; padding-right: 0px;">
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnContentInsert" runat="server" Text="add to contents" UseSubmitBehavior="false" CssClass="btn" Width="150px" data-controlname="btnContentInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbcontents') || btnAdd_Clicked('lbFormula', 'lbcontents')" /></div>
                    <div>
                        <asp:ListBox ID="lbcontents" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbcontents"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnContentUp" runat="server" Text="Up" CssClass="btn" OnClientClick="return btnUp_Clicked('lbcontents')" />
                        <asp:Button ID="btnContentDown" runat="server" Text="Down" CssClass="btn " OnClientClick="return btnDown_Clicked('lbcontents')" />
                        <asp:Button ID="btnContentDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbcontents')" />
                    </div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnCriteriaInsert" runat="server" Text="add to criterias" CssClass="btn " Width="150px" data-controlname="btnCriteriaInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbcriteria')" /></div>
                    <div>
                        <asp:ListBox ID="lbcriteria" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbcriteria"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnCriteriaDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbcriteria')" /></div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnSortonInsert" runat="server" Text="add to sort on" CssClass="btn " Width="150px" data-controlname="btnSortonInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbsorton') || btnAdd_Clicked('lbFormula', 'lbsorton')" /></div>
                    <div>
                        <asp:ListBox ID="lbsorton" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbsorton"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnSortonUp" runat="server" Text="Up" CssClass="btn " OnClientClick="return btnUp_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonDown" runat="server" Text="Down" CssClass="btn " OnClientClick="return btnDown_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbsorton')" />
                        <asp:Button ID="btnSortonAsc" runat="server" Text="Asc." CssClass="btn" OnClientClick="return btnSortonAsc_Clicked()" />
                        <asp:Button ID="btnSortonDesc" runat="server" Text="Desc." CssClass="btn" OnClientClick="return btnSortonDesc_Clicked()" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGroupByInsert" runat="server" Text="add to group by" CssClass="btn " Width="150px" data-controlname="btnGroupByInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbhiden') || btnAdd_Clicked('lbFormula', 'lbhiden')" /></div>
                    <div>
                        <asp:ListBox ID="lbhiden" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbhiden"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                    <asp:Button ID="btnGroupbyUp" runat="server" Text="Up" CssClass="btn " OnClientClick="return btnGroupby_Up_Clicked()" />
                        <asp:Button ID="btnGroupbyDown" runat="server" Text="Down" CssClass="btn " OnClientClick="return btnGroupby_Down_Clicked()" />
                        <asp:Button ID="btnGroupbyDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbhiden')" />
                        <asp:Button ID="btnGroupbyReset" runat="server" Text="Reset" CssClass="btn " OnClientClick="return btnReset_Clicked('lbhiden')" />
                    </div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnSumInsert" runat="server" Text="add to report total" CssClass="btn " Width="150px" data-controlname="btnSumInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbsum') || btnAdd_Clicked('lbFormula', 'lbsum')" /></div>
                    <div>
                        <asp:ListBox ID="lbsum" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbsum"></asp:ListBox>
                    </div>
                    <div>
                        <asp:Button ID="btnSumDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbsum')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGrouptotalInsert" runat="server" Text="add to group total" CssClass="btn " Width="150px" data-controlname="btnGrouptotalInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbgrouptotal') || btnAdd_Clicked('lbFormula', 'lbgrouptotal')" /></div>
                    <div>
                        <asp:ListBox ID="lbgrouptotal" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbgrouptotal"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGrouptotalDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbgrouptotal')" /></div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnAvgInsert" runat="server" Text="add to report avg" CssClass="btn " Width="150px" data-controlname="btnAvgInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbavg') || btnAdd_Clicked('lbFormula', 'lbavg')" /></div>
                    <div>
                        <asp:ListBox ID="lbavg" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbavg"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnAvgDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbavg')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGroupavgInsert" runat="server" Text="add to group avg" CssClass="btn " Width="150px" data-controlname="btnGroupavgInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbgroupavg') || btnAdd_Clicked('lbFormula', 'lbgroupavg')" /></div>
                    <div>
                        <asp:ListBox ID="lbgroupavg" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbgroupavg"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGroupavgDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbgroupavg')" /></div>
                </div>
            </div>
            <div class="row">
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnRpcountInsert" runat="server" Text="add to report count" CssClass="btn " Width="150px" data-controlname="btnRpcountInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbrpcount') || btnAdd_Clicked('lbFormula', 'lbrpcount')" /></div>
                    <div>
                        <asp:ListBox ID="lbrpcount" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbrpcount"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnRpcountDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbrpcount')" /></div>
                </div>
                <div class='col-md-6' style="min-width: 110px; max-width: 400px; padding-left: 0px; padding-right: 0px;">
                    <div style="margin-bottom: 5px;">
                        <asp:Button ID="btnGroupcountInsert" runat="server" Text="add to group count" CssClass="btn " Width="150px" data-controlname="btnGroupcountInsert" OnClientClick="return btnAdd_Clicked('lbAllColumns', 'lbgroupcount') || btnAdd_Clicked('lbFormula', 'lbgroupcount')" /></div>
                    <div>
                        <asp:ListBox ID="lbgroupcount" runat="server" Style="width: 90%; height: 200px;" SelectionMode="Multiple" data-controlname="lbgroupcount"></asp:ListBox>
                    </div>
                    <div style="margin-bottom: 20px;">
                        <asp:Button ID="btnGroupcountDelete" runat="server" Text="Delete" CssClass="btn " OnClientClick="return btnRemove_Clicked('lbgroupcount')" /></div>
                </div>
            </div>
        </div>
    </div>
    <div id="delconfirm" class="modal fade" role="dialog" aria-hidden="true" style="margin-top: 50px" >
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-body">
                    Are you sure to delete?
                </div>
                <div class="modal-footer">
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" CssClass="btn btn-primary" ValidationGroup="sub" Visible="false" />
                    <button type="button" data-dismiss="modal" class="btn">Cancel</button>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('#ContentPlaceHolder1_lbFormula').focus(function (e) {
                e.preventDefault();
                e.target.focus({ preventScroll: true });
            })
            $(document).bind('focus', function (e) {
                e.preventDefault();
            });
            //$('#el').focus((e) => {
            //    e.preventDefault();
            //e.target.focus({preventScroll: true});
            //})
            //$('[data-controlname=txtReportName]').change(txtReportNameChanged()).blur(txtReportNameChanged());
            $('[data-controlname=txtReportName]').blur(txtReportNameChanged());
            $('[data-toggle=tab]').click(function () {
                unselectAll('lbAllColumns');
                unselectAll('lbFormula');
                if ($(this).parent().hasClass('active')) {
                    $($(this).attr("href")).toggleClass('active');
                }
                if ($(this).attr("href") == "#tabAllColumns") {
                    $('[data-controlname=btnCriteriaInsert]').removeClass("disabled");
                    //$('[data-controlname=btnSumInsert]').removeClass("disabled");
                    //$('[data-controlname=btnGrouptotalInsert]').removeClass("disabled");
                    //$('[data-controlname=btnAvgInsert]').removeClass("disabled");
                    //$('[data-controlname=btnGroupavgInsert]').removeClass("disabled");
                } else {
                    $('[data-controlname=btnCriteriaInsert]').addClass("disabled");
                    //$('[data-controlname=btnSumInsert]').addClass("disabled");
                    //$('[data-controlname=btnGrouptotalInsert]').addClass("disabled");
                    //$('[data-controlname=btnAvgInsert]').addClass("disabled");
                    //$('[data-controlname=btnGroupavgInsert]').addClass("disabled");
                };
            })
            $('[data-controlname=lbFormula]').change(function () {
                lbFormula_selectionChanged();
            });
            $('[data-controlname=ddlQueryName]').change(function () {
                unselectAll('lbAllColumns');
                unselectAll('lbFormula');
            });
            $("#txtAllColumnsFilter").keyup(txtAllColumnsFilter_invoked);
        });
    </script>
    
        <div id="templatemgt" class="modal fade" role="dialog" aria-hidden="true" style="margin-top: 50px">
        <div class="modal-dialog" style="width:760px">
            <div class="modal-content">
                <div class="modal-header"><h4 class="modal-title">Pivotable Template Management</h4></div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-xs-3" ><label>Upload template</label></div>
                        <div class="col-xs-9">
                            <asp:FileUpload ID="fu_exceltempletea" runat="server" CssClass="btn form-control btn-block" Style="width: 300px" />
                            <input type="button" id="btn_uploadTemplateb" value="Upload" runat="server" Text="Add Template" class="btn form-control btn-block" style="width:300px" onserverclick="btn_uploadTemplate_ServerClick"/>
                            <label runat="server" ID="lt_filenamec" class="form-control btn-block" style="font-size:13px"></label>
                            <label runat="server" ID="lt_popMessage" class="control-label" style=" font-size:13px; color:red">error.</label>
                        </div>
                    </div>
                </div>
                <div class="modal-footer"><button type="button" data-dismiss="modal" class="btn btn-primary">Close</button></div>
            </div>
        </div>
    </div>
    <asp:Literal ID="lblJavascript" runat="server" />
</asp:Content>
<asp:Content ID="contentNonFormItems" ContentPlaceHolderID="cp_nonFormControls" runat="server">
    <script type="text/javascript">
        function refreshColumns() {
            jQuery.ajax({
                url: "rpexcel.aspx?action=refreshDlgColumns",
                type: "POST",
                success: function (data, textStatus, jqXHR) {
                    if (data.fields != null) {
                        var currentitem = null;
                        $("#divColumnNames .SelectionBlock").remove();
                        for (i = 0; i < data.fields.length; i++) {
                            currentitem = data.fields[i];
                            $("#divColumnNames").append($("<div></div>").addClass("SelectionBlock").attr("data-text", currentitem.ColumnName).text(currentitem.DisplayName));
                        }
                        $(".SelectionBlock").draggable({
                            opacity: 0.7,
                            helper: "clone",
                            scroll: false,
                            containment: '#divSQLText',
                            appendTo: '#divSQLText',
                            drag: function (event, ui) {
                                var col = Math.floor((ui.position.left - 295) / 8);
                                var row = Math.floor((ui.position.top - 34) / 23);
                                var charpos = getTextPosition('txtSQLText', col, row);
                                //$("#txtdragstatus").text("left = " + ui.position.left + " top = " + ui.position.top + " col = " + col + " row = " + row + " pos = " + charpos);
                                if (col > 0) {
                                    $("#txtSQLText").textrange('setcursor', charpos);
                                }
                            }
                        });
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
        }
        function paramForumulaValidateText() {
            var queryid = $('[data-controlname=ddlQueryName]').val();
            var fieldname = $('#txtFormulaName').val();
            var fieldtext = $('#txtSQLText').text();
            if ((fieldtext == null) || (fieldtext == "")) {
                fieldtext = $('#txtSQLText').val(); // use val only when text is not available (like in Chrome) for extra protection
            }
            jQuery.ajax({
                url: "rpexcel.aspx?action=checkFieldText",
                type: "POST",
                data: {
                    queryid: queryid,
                    fieldname: fieldname,
                    fieldtext: fieldtext
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.indexOf('OK') == 0) {
                        alert("The query appears to be okay.");
                    } else if (data.indexOf('DOCTYPE') > 0) {
                        alert("Session expired. Please login again.");
                    } else {
                        alert(data);
                    }
                }
            });
            return false;
        }
        function paramForumulaNameChanged() {
            id = $('#hid_formulaidx').val();
            fieldname = $('#txtFormulaName').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=updateFieldName",
                type: "POST",
                data: {
                    id: id,
                    fieldname: fieldname
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        // do not update controls to avoid changing cursor positions
                        //$('#hid_formulaidx').val(data.Idx);
                        //$('#txtFormulaName').val(data.FieldName);
                        //$('#txtSQLText').text(data.RenderText);
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function paramForumulaTextChanged() {
            var id = $('#hid_formulaidx').val();
            var queryid = $('[data-controlname=ddlQueryName]').val();
            var fieldname = $('#txtFormulaName').val();
            var fieldtext = $('#txtSQLText').text();
            if ((fieldtext == null) || (fieldtext == "")) {
                fieldtext = $('#txtSQLText').val(); // use val only when text is not available (like in Chrome) for extra protection
            }
            jQuery.ajax({
                url: "rpexcel.aspx?action=updateFieldText",
                type: "POST",
                data: {
                    id: id,
                    queryid: queryid,
                    fieldname: fieldname,
                    fieldtext: fieldtext
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        // do not update controls to avoid changing cursor positions
                        //$('#hid_formulaidx').val(data.Idx);
                        //$('#txtFormulaName').val(data.FieldName);
                        //$('#txtSQLText').text(data.RenderText);
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function refreshdlg(paramid) {
            var id = paramid;
            if (id == null) { id = $('#hid_formulaidx').val(); }
            jQuery.ajax({
                url: "rpexcel.aspx?action=reloadFormulaDlg",
                type: "POST",
                data: {
                    id: id
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.Idx != null) {
                        $('#hid_formulaidx').val(data.Idx);
                        $('#txtFormulaName').val(data.FieldName);
                        $('#txtSQLText').text(data.RenderText);
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
        function formulaSaveChange() {
            var id = $('#hid_formulaidx').val();
            var queryid = $('[data-controlname=ddlQueryName]').val();
            var fieldname = $('#txtFormulaName').val();
            //var fieldtext = $('#txtSQLText').text();
            //if ((fieldtext == null) || (fieldtext == "")) {
            //    fieldtext = $('#txtSQLText').val(); // use val only when text is not available (like in Chrome) for extra protection
            //}
            //v1.8.8 Alex - Must use .val() in all cases, or else it does not update.
            var fieldtext = $('#txtSQLText').val();
            jQuery.ajax({
                url: "rpexcel.aspx?action=formulaSaveChange",
                type: "POST",
                data: {
                    id: id,
                    queryid: queryid,
                    fieldname: fieldname,
                    fieldtext: fieldtext
                },
                success: function (data, textStatus, jqXHR) {
                    if (data.fields != null) {
                        $('[data-controlname=lbFormula] option').remove();
                        var ctrl = $('[data-controlname=lbFormula]');
                        for (i = 0; i < data.fields.length; i++) {
                            //v1.8.8 Alex 2018.10.03 - Disallow adding to subtotal if IsNumeric is false - Begin
                            if (data.IsNumeric[i]) {
                                ctrl.append($("<option></option>").attr("value", data.fields[i]).attr("numeric", "true").text(data.fields[i]));
                            } else {
                                ctrl.append($("<option></option>").attr("value", data.fields[i]).attr("numeric", "false").text(data.fields[i]));
                            }
                            console.log(i + 'th: ' + data.IsNumeric[i]);
                            //v1.8.8 Alex 2018.10.03 - Disallow adding to subtotal if IsNumeric is false - End
                        }
                        $("#dlgFormula").modal("hide");

                        //v1.8.8 Alex 2018.10.03 - Disallow adding to subtotal if IsNumeric is false - Begin
                        ctrl.change(function () {
                            console.log($(this).val());
                            if ($('[data-controlname=lbFormula] option:selected[numeric="false"]').size() > 0) {
                                console.log('Disabled!');
                                //Override onclick property
                                $('[data-controlname=btnSumInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbsum')");
                                $('[data-controlname=btnAvgInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbavg')");
                                $('[data-controlname=btnRpcountInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbrpcount')");

                                $('[data-controlname=btnGrouptotalInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgrouptotal')");
                                $('[data-controlname=btnGroupavgInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupavg')");
                                $('[data-controlname=btnGroupcountInsert]').addClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupcount')");
                            } else {
                                console.log('Enabled!');
                                $('[data-controlname=btnSumInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbsum') || btnAdd_Clicked('lbFormula', 'lbsum')");;
                                $('[data-controlname=btnAvgInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbavg') || btnAdd_Clicked('lbFormula', 'lbavg')");
                                $('[data-controlname=btnRpcountInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbrpcount') || btnAdd_Clicked('lbFormula', 'lbrpcount')");

                                $('[data-controlname=btnGrouptotalInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgrouptotal') || btnAdd_Clicked('lbFormula', 'lbgrouptotal')");
                                $('[data-controlname=btnGroupavgInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupavg') || btnAdd_Clicked('lbFormula', 'lbgroupavg')");
                                $('[data-controlname=btnGroupcountInsert]').removeClass("disabled").attr("onclick", "return btnAdd_Clicked('lbAllColumns', 'lbgroupcount') || btnAdd_Clicked('lbFormula', 'lbgroupcount')");
                            }
                        });
                        //v1.8.8 Alex 2018.10.03 - Disallow adding to subtotal if IsNumeric is false - End
                    } else {
                        if (data.indexOf('DOCTYPE') > 0) {
                            alert("Session expired. Please login again.");
                        } else {
                            alert(data);
                        }
                    }
                }
            });
            return false;
        }
    </script>
    <div class="modal fade" id="dlgFormula" tabindex="-1" role="dialog" aria-labelledby="dlgCaption" aria-hidden="true" style="margin-top: 50px; overflow: hidden">
        <div class="modal-dialog" style="width: 80%; min-width: 1280px">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="dlgCaption">
                        Edit Formula</h4>
                </div>
                <div class="modal-body">
                    <div class='row' style="margin-bottom: 15px">
                        <div class="col-xs-1">
                            <label class="col-md-1 control-label" style="white-space: nowrap">Field name</label></div>
                        <div class="col-xs-11">
                            <input type="text" id="txtFormulaName" class="form-control" onchange="return paramForumulaNameChanged();" /><input type="hidden" id="hid_formulaidx" value="0" /></div>
                    </div>
                    <div class='row' style="margin-bottom: 15px">
                        <div class="col-xs-1">
                            <label class="col-md-1 control-label">Formula</label></div>
                        <div class="col-xs-11" id="div_formula" style="max-height: 300px; min-width: 1114px">
                            <div class="row" style="padding-left: 20px">
                                <div class="SelectionBlock btn" style="width: 18%" data-text="CASE WHEN THEN ELSE END" title="CASE &lt;field to check&gt; ... &lt;Mapping Case&gt; ... ELSE &lt;Value when no mapping case matched&gt; END">
                                    Mapping</div>
                                <div class="SelectionBlock btn" style="width: 18%" data-text="WHEN THEN" title="WHEN &lt;value to be mapped&gt; THEN &lt;value when matched&gt;">
                                    Mapping Case</div>
                                <div class="SelectionBlock btn" style="width: 18%" data-text="ISNULL( , NULL)" title="ISNULL(&lt;espression to check&gt;, &lt;value to be display if espression evaluated to NULL&gt;)">
                                    Empty Value</div>
                                <div class="SelectionBlock btn" style="width: 18%" data-text="ROUND( , 2)" title="ROUND(&lt;espression to apply rounding to&gt;,&lt;round to decimal place (can be negative if need to round to left hand side of decimal point)&gt;)">
                                    Rounding</div>
                                <div class="SelectionBlock btn" style="width: 18%" data-text="RTRIM( )" title="RTRIM(&lt;espression that has to remove space character(s) in the end&gt;)">
                                    Trim trailling space</div>
                            </div>
                            <div id="divColumnNames" class='col-md-4' style="min-width: 210px; max-width: 280px; padding-left: 5px; max-height: 300px; overflow: auto">
                            </div>
                            <div id="divSQLText">
                                <textarea id="txtSQLText" rows="30" cols="120" style="width: 800px; height: 300px; white-space: pre; word-wrap: normal; overflow: scroll;"></textarea>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn pull-left" onclick="return refreshdlg();">
                        Refresh</button>
                    <button type="button" class="btn pull-left" onclick="return paramForumulaValidateText();">
                        Validate</button>
                    <button type="button" class="btn pull-left btn-primary" onclick="return formulaSaveChange();">
                        Save and Close</button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            refreshColumns();
            $('#dlgFormula').on('show.bs.modal', function () {
                refreshdlg();
            });
            $("#txtSQLText").droppable({
                accept: ".SelectionBlock",
                tolerance: "fit",
                drop: function (event, ui) {
                    //$("#txtdropstatus").text("left = " + ui.position.left + " top = " + ui.position.top);
                    if (ui.draggable.hasClass("btn")) {
                        $(this).textrange('replace', ' ' + ui.draggable.attr("data-text") + ' ').trigger('updateInfo').focus();
                    } else {
                        $(this).textrange('replace', ' [' + ui.draggable.attr("data-text") + '] ').trigger('updateInfo').focus();
                    }
                    //paramForumulaTextChanged();
                }
            });
        });
    </script>
    <script src="../assets/javascripts/plugins/textrange/jquery-textrange.js" type="text/javascript"></script>
</asp:Content>
