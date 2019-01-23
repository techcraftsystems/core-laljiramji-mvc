jq(function() {
    jq('.tabs').tabs();

    var _oldShow = jq.fn.show;

    jq.fn.show = function(speed, oldCallback) {
        return jq(this).each(function() {
            var obj = jq(this),
                newCallback = function() {
                    if (jq.isFunction(oldCallback)) {
                        oldCallback.apply(obj);
                    }
                    obj.trigger('afterShow');
                };

            // you can trigger a before show if you want
            obj.trigger('beforeShow');

            // now use the old function to show the element passing the new callback
            _oldShow.apply(obj, [speed, newCallback]);
        });
    }

    jq('ul .pymt').click(function(){
        if(jq("#paymentTbl").data('loaded') == 0){
            jq("#paymentTbl").data('loaded', 1);
            GetCustomerPayments();
        }
    });

    jq(".get-summary a").click(function() {
        GetStationSummary(true);
    });

    jq(".get-payments a").click(function() {
        GetCustomerPayments(true);
    });

    String.prototype.toAccounting = function() {
        return parseFloat(this).toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
    };

    function GetStationSummary(Notify) {
        jq.ajax({
            dataType: "json",
            url: '/Stations/GetStationsSummary',
            data: {
                "date1": jq("#startDate").val(),
                "date2": jq("#stopDate").val(),
                "stats": jq(".select-dropdown").val()
            },
            beforeSend: function() {
                if (Notify) {
                    $('body').removeClass('loaded');
                }
            },
            success: function(results) {
                var sale = 0.0;
                var cash = 0.0;
                var invs = 0.0;
                var visa = 0.0;
                var mpes = 0.0;
                var poss = 0.0;
                var exps = 0.0;
                var disc = 0.0;
                var summ = 0.0;

                jq('#summaryTbl tbody').empty();
                jq('#summaryTbl tfoot').empty();

                jq.each(results, function(i, st) {
                    var row = "<tr>";
                    row += "<th><b>" + (i + 1) + ". <a href='stations/" + st.station.code + "'>" + st.station.name + "</a></b></th>";
                    row += "<td class='red-text bold-text'>" + st.sale.toString().toAccounting() + "</td>";
                    row += "<td>" + st.cash.toString().toAccounting() + "</td>";
                    row += "<td>" + st.invoice.toString().toAccounting() + "</td>";
                    row += "<td>" + st.visa.toString().toAccounting() + "</td>";
                    row += "<td>" + st.mpesa.toString().toAccounting() + "</td>";
                    row += "<td>" + st.pos.toString().toAccounting() + "</td>";
                    row += "<td>" + st.expense.toString().toAccounting() + "</td>";
                    row += "<td>" + st.discount.toString().toAccounting() + "</td>";
                    row += "<td class='red-text bold-text'>" + st.summary.toString().toAccounting() + "</td>";
                    row += "<td class='blue-text'>" + (st.sale - st.summary).toString().toAccounting() + "</td>";
                    row += "</tr>";

                    sale += st.sale;
                    cash += st.cash;
                    invs += st.invoice;
                    visa += st.visa;
                    mpes += st.mpesa;
                    poss += st.pos;
                    exps += st.expense;
                    disc += st.discount;
                    summ += st.summary;

                    jq('#summaryTbl tbody').append(row);
                })

                var footr = "<tr>";
                footr += "<td></td>";
                footr += "<td><b>" + sale.toString().toAccounting() + "</b></td>";
                footr += "<td>" + cash.toString().toAccounting() + "</td>";
                footr += "<td>" + invs.toString().toAccounting() + "</td>";
                footr += "<td>" + visa.toString().toAccounting() + "</td>";
                footr += "<td>" + mpes.toString().toAccounting() + "</td>";
                footr += "<td>" + poss.toString().toAccounting() + "</td>";
                footr += "<td>" + exps.toString().toAccounting() + "</td>";
                footr += "<td>" + disc.toString().toAccounting() + "</td>";
                footr += "<td><b>" + summ.toString().toAccounting() + "</b></td>";
                footr += "<td>" + (sale - summ).toString().toAccounting() + "</td>";
                footr += "</tr>";

                jq('#summaryTbl tfoot').append(footr);

            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                $('body').addClass('loaded');
            }
        });
    }

    function GetCustomerPayments() {
        var stations = jq('#paymentSel').closest('div').find('select').val();

        jq.ajax({
            dataType: "json",
            url: '/Stations/GetCustomerPayments',
            data: {
                "date1": jq("#paymentStartDate").val(),
                "date2": jq("#paymentStopDate").val(),
                "stations": stations.toString(),
            },
            beforeSend: function() {
                $('body').removeClass('loaded');
            },
            success: function(results) {
                var total = 0.0;

                jq('#paymentTbl tbody').empty();
                jq('#paymentTbl tfoot').empty();

                jq.each(results, function(i, pt) {
                    var row = "<tr>";
                    row += "<td>" + (i + 1) + "</td>";
                    row += "<td>" + pt.date + "</td>";

                    if (pt.type == 0) {
                        row += "<td><a class='blue-text' href='/customers/" + pt.station.code + "/" + pt.customer.id + "'>" + pt.customer.name + "</a></td>";
                    }
                    else if (pt.type == 3) {
                        row += "<td><a class='blue-text' href='/accounts/" + pt.station.code + "/" + pt.customer.id + "'>" + pt.customer.name + "</a></td>";
                    }
                    else {
                        row += "<td><a class='blue-text' href='/accounts/" + pt.station.code + "/mpesa'>" + pt.customer.name + "</a></td>";
                    }

                    row += "<td><a class='blue-text' href='/core/stations/" + pt.station.code + "'>" + pt.station.name.toUpperCase() + "</a></td>";
                    row += "<td>" + pt.receipt + "</td>";
                    row += "<td>" + pt.cheque + "</td>";
                    row += "<td class='bold-text' style='text-align:right;padding-right:15px;'>" + pt.amount.toString().toAccounting() + "</td>";
                    row += "<td>" + pt.notes + "</td>";
                    row += "<td><a class='material-icons tiny-box grey-text right'>border_color</a></td>";
                    row += "</tr>";

                    total += pt.amount;

                    jq('#paymentTbl tbody').append(row);
                })

                var footr = "<tr>";
                footr += "<td>&nbsp;</td>";
                footr += "<td class='bold-text' colspan='5'>PAYMENT SUMMARY</td>";
                footr += "<td class='bold-text' style='text-align:right;padding-right:15px;'>" + total.toString().toAccounting() + "</td>";
                footr += "</tr>";

                jq('#paymentTbl tfoot').append(footr);

            },
            error: function(xhr, ajaxOptions, thrownError) {
                console.log(xhr.status);
                console.log(thrownError);
            },
            complete: function() {
                $('body').addClass('loaded');
            }
        }); 
    }

    GetStationSummary(false);
});