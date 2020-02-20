<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="D3Chart.ascx.cs" Inherits="QueryReport.Controls.D3Chart" %>
<div id="well<%=this.cID %>" class="well well-lg col-md-6">
    <!-- e.g. well0 -->
    <svg id="char<%=this.cID%>" />
    <div id="tooltip_char<%=this.cID%>" class="ctooltip hidden">
        <p><span id="value_char<%=this.cID%>"></span></p>
    </div>
    <!-- e.g. char0 -->
    <%--Add access control (add LinkButton on server-side)--%>
    <asp:LinkButton runat="server" CssClass="btn btn-danger btn-sm pull-right closeButton" OnClick="RemoveChart" AutoPostBack="true" CausesValidation="false">
        <i class="fa fa-times" data-toggle="tooltip" data-placement="top" title="Remove chart from dashboard"></i>
    </asp:LinkButton>
    <input class="sort_switch bootstrap-switch" type="checkbox" data-size="mini" data-on-text="A-Z"
        data-off-text="Descending" onchange='sort<%= (this.Type == 0) ? "Pie" : (this.Type == 1) ? "Donut" : "Bars" %>("char<%=this.cID%>", <%=this.chart_data%>, "<%=this.cID%>")' />
    <script type="text/javascript">
        $('[data-toggle="tooltip"]').tooltip();
        $(".sort_switch").bootstrapSwitch();
        // Data for the pie chart
        //var chart_0_data = [];
        //var chart_0_title = "", views = "";

        var sort<%=this.cID%> = true;
            
        // Draw the chart
        function updateChart() {
                 <% if (this.Type == 0)
                    { %> 
            drawPie("char<%=this.cID%>", <%=this.chart_data%>, "<%=this.chart_title%>");
            <% }
                    else if (this.Type == 1)
                    { %>
            drawDonut("char<%=this.cID%>", <%=this.chart_data%>, "<%=this.chart_title%>"); // TODO: may change to data binding
            <% }
                    else if (this.Type == 2)
                    { %>
            drawBars("char<%=this.cID%>", <%=this.chart_data%>, "<%=this.chart_title%>", "<%=this.bar_x_axis_label%>", "<%=this.bar_y_axis_label%>");
            <% }
                    else if (this.Type == 3)
                    { %>
            drawLine("char<%=this.cID%>", <%=this.chart_data%>, "<%=this.chart_title%>");
            <% } %>
        }

        function drawDonut(svg_id, data, title) {
            //Redraw for zoom
            function redraw() {
                d3.select("#" + svg_id).select('g').attr("transform", "translate(" + d3.event.transform.x + " " + d3.event.transform.y + ")" + 
                                      " scale(" + d3.event.transform.k + ")");
            }

            var total = d3.sum(data, function(d) { 
                return d.value; 
            });
            data.forEach(function(d) {
                d.percentage = Math.round(d.value / total * 100) + "%";
            });

            let width = 500, height = 400;
            var svg = d3.select("#" + svg_id).attr("width", width).attr("height", height);
            var gradPie = {};

            var pie = d3.pie().sort(function(x, y){
                return d3.ascending(x.value, y.value);
            }).value(function (d) { return d.value; });

            createGradients = function (defs, colors, r) {
                var gradient = defs.selectAll('.gradient')
                    .data(colors).enter().append("radialGradient")
                    .attr("id", function (d, i) { return "gradient" + i; })
                    .attr("gradientUnits", "userSpaceOnUse")
                    .attr("cx", "0").attr("cy", "0").attr("r", r).attr("spreadMethod", "pad");

                gradient.append("stop").attr("offset", "0%").attr("stop-color", function (d) { return d; });

                gradient.append("stop").attr("offset", "30%")
                    .attr("stop-color", function (d) { return d; })
                    .attr("stop-opacity", 1);

                gradient.append("stop").attr("offset", "70%")
                    .attr("stop-color", function (d) { return "black"; })
                    .attr("stop-opacity", 1);
            }

            gradPie.draw = function (id, data, cx, cy, r) {
                var gPie = d3.select("#" + id).append("g")
                    .attr("transform", "translate(" + cx + "," + cy + ")");

                createGradients(gPie.append("defs"), d3.schemeSet1.concat(d3.schemeSet2).concat(d3.schemeSet3), 2.5 * r);

                gPie.selectAll("path").data(pie(data))
                    .enter().append("path").attr("fill", function (d, i) { return "url(#gradient" + i + ")"; })
                    .attr("d", d3.arc()
                .outerRadius(r - 25)
                .innerRadius(r - 80)
            )
                     .on("mouseover", function(d, i) {
                         var xPosition = width / 2;
                         var yPosition = height / 2;
                         d3.select("#tooltip_" + svg_id)
                           .style("left", xPosition + "px")
                           .style("top", yPosition + "px")
                           .style("background-color", d3.schemeSet1.concat(d3.schemeSet2).concat(d3.schemeSet3)[i])
                           .select("#value_" + svg_id)
                           .text(d.data.label + ':\t' + d.value + ' ('+ d.data.percentage + ')')
                           .style("color", d3.hsl(d3.schemeSet1.concat(d3.schemeSet2).concat(d3.schemeSet3)[i]).l < 0.5 ? "white" : "black")
                           .style("font-weight", "bold");
                         d3.select("#tooltip_" + svg_id).classed("hidden", false);
                     })
                     .on("mouseout", function() {
                         d3.select("#tooltip_" + svg_id).classed("hidden", true);
                     })
                    .each(function (d) { this._current = d; });

                // place labels on the chart
                gPie.selectAll("text").data(pie(data))
                    .enter()
                    .append("text")
                    .attr("class", "percentage")
                    .attr("transform", function (d) {
                        d.innerRadius = r / 2.0;
                        d.outerRadius = r;   // text will be inserted in the center of the current section

                        return "translate(" + d3.arc()
                            .outerRadius(r + 60)
                            .innerRadius(0)
                            .padAngle(0.03).centroid(d) + ")"
                    })
                    .attr("dy", 5) 
                    .style("text-anchor", "start")
                    .attr("text-anchor", "middle")
                    .style("font-size", "x-small")
                    .style("font-weight", "bold")
                    .style("fill", "white")
                    .text(function (d, i) { return d.data.percentage; });   // d.data - our data item, assigned to the current section. "label" is a part of our data object

                // place labels on the chart
                gPie.selectAll().data(pie(data))
                    .enter()
                    .append("text")
                    .attr("class", "label")
                    .attr("transform", function (d) {
                        d.innerRadius = r / 2.0;
                        d.outerRadius = r;   // text will be inserted in the center of the current section

                        return "translate(" + d3.arc()
                            .outerRadius(r + 120)
                            .innerRadius(0)
                            .padAngle(0.03).centroid(d) + ") " +
                            "rotate(" + (180 / Math.PI * (d.startAngle + d.endAngle) / 2 - 90) + ")"
                    })
                    .attr("dy", 5) 
                    .style("text-anchor", "start")
                    .attr("text-anchor", "middle")
                    .style("font-size", "x-small")
                    .style("font-weight", "bold")
                    .text(function (d, i) { return d.data.label; });   // d.data - our data item, assigned to the current section. "label" is a part of our data object
                     
                d3.select("#" + id)
                    .append("text")
                    .text(title)
                    .attr("transform",
                          "translate(" + (cx/2) + "," + 
                                         20 + ")")
                    .attr("class", "title");
                
                d3.select("#" + id).call(d3.zoom().scaleExtent([0.15,3])
                    .on("zoom", redraw));
            }

            svg.append("g").attr("id", "donut_" + svg_id);
            gradPie.draw("donut_" + svg_id, data, width / 2, height / 2 + 30, 160);

            function positionLabels(pie, section, sectionDisplayType) {
                labels["dimensions-" + section] = [];

                // get the latest widths, heights
                var labelGroups = d3.selectAll("." + pie.cssPrefix + "labelGroup-" + section);
                labelGroups.each(function(d, i) {
                    var mainLabel  = d3.select(this).selectAll("." + pie.cssPrefix + "segmentMainLabel-" + section);

                    labels["dimensions-" + section].push({
                        mainLabel:  (mainLabel.node() !== null) ? mainLabel.node().getBBox() : null,
                    });
                });

                var singleLinePad = 5;
                var dims = labels["dimensions-" + section];
                switch (sectionDisplayType) {
                    case "label-value1":
                        d3.selectAll("." + pie.cssPrefix + "segmentValue-" + section)
                            .attr("dx", function(d, i) { return dims[i].mainLabel.width + singleLinePad; });
                        break;
                    case "label-value2":
                        d3.selectAll("." + pie.cssPrefix + "segmentValue-" + section)
                            .attr("dy", function(d, i) { return dims[i].mainLabel.height; });
                        break;
                }
            }
        }

        function arcTween(a) {
            var i = d3.interpolate(this._current, a);
            this._current = i(0);
            return function(t) {
                arc = d3.arc()
                .outerRadius(160 - 25)
                .innerRadius(160 - 80);
                return arc(i(t));
            };
        }
        function transformTweenPercentage(a) {
            var i = d3.interpolate(this._current, a);
            this._current = i(0);
            return function(d) { d.innerRadius = 160 / 2.0;
                var r = 160;
                d.outerRadius = r;   // text will be inserted in the center of the current section
                return "translate(" + d3.arc()
                .outerRadius(r + 60)
                .innerRadius(0)
                .padAngle(0.03).centroid(i(d)) + ")"
            }
        }
        function transformTweenLabel(a) {
            var i = d3.interpolate(this._current, a);
            this._current = i(0);
            return function(d) { d.innerRadius = 160 / 2.0;
                var r = 160;
                d.outerRadius = r;   // text will be inserted in the center of the current section
                return "translate(" + d3.arc()
                  .outerRadius(r + 120)
                  .innerRadius(0)
                  .padAngle(0.03).centroid(i(d)) + ") " +
                            "rotate(" + (180 / Math.PI * (i(d).startAngle + i(d).endAngle) / 2 - 90) + ")";
            }
        }
        function sortDonut(id, data, ctrid) {
            eval('sort'+ctrid+' ^= 1;');
            var pie1;
            gPie = d3.select("#" + id);
            if (eval('!sort'+ctrid)) {
                pie1 = d3.pie().sort(function(x, y){
                    return d3.descending(x.label, y.label);
                }).value(function (d) { return d.value; });
            } else {
                pie1 = d3.pie().sort(function(x, y){
                    return d3.ascending(x.value, y.value);
                }).value(function (d) { return d.value; });
            }

            gPie.selectAll("path").data(pie1(data));

            gPie.selectAll("path")
            .transition(
              d3.transition()
            .duration(1000)
            .ease(d3.easeLinear)
            ).attrTween("d", arcTween); // redraw the arcs

            gPie.selectAll(".percentage").data(pie1(data));


            gPie.selectAll(".percentage")
            .transition(
              d3.transition()
            .duration(1000)
            .ease(d3.easeLinear)
            ).attrTween("transform", transformTweenPercentage); // redraw the arcs

            gPie.selectAll(".label").data(pie1(data));

            gPie.selectAll(".label")
            .transition(
              d3.transition()
            .duration(1000)
            .ease(d3.easeLinear)
            ).attrTween("transform", transformTweenLabel); // redraw the arcs
        }


        function drawPie(svg_id, data, title) {
            let pie = new d3pie(svg_id, {
                size: {
                    canvasWidth: 500,
                    canvasHeight: 400
                },
                header: {
                    title: {
                        text: title
                    }
                },
                data: {
                    sortOrder: "value-asc",
                    content: data,
                    lines: {
                        style: "straight"
                    }
                },
                effects: {
                    load: {
                        effect: "none"
                    }
                }
            });
        }
        function sortPie(id, data) {
        }

        function drawBars(svg_id, data, title, xlabel, ylabel) {
            var margin = {top: 40, right: 20, bottom: 40, left: 80},
            width = 500 - margin.left - margin.right,
            height = 400 - margin.top - margin.bottom;

            var x = d3.scaleLinear()
                .range([0, width]);

            var y = d3.scaleBand().rangeRound([0, height]).padding(0.3);

            var xAxis = d3.axisBottom(x);

            var yAxis = d3.axisLeft(y)
                .tickSize(0)
                .tickPadding(6);

            var svg =  d3.select("#" + svg_id)
                .attr("width", width + margin.left + margin.right)
                .attr("height", height + margin.top + margin.bottom)
                .append("g")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

            function draw(data) {
                var s = d3.extent(data, function(d) { return d.value; });
                s[0] = s[0] > 0 ? 0 : s[0];
                x.domain(s).nice();
                y.domain(data.sort(function(a, b){
                    return d3.descending(a.value, b.value);
                }).map(function(d) { return d.label; }));

                svg.selectAll(".bar")
                    .data(data)
                    .enter().append("rect")
                    .attr("class", function(d) { return "bar bar--" + (d.value < 0 ? "negative" : "positive"); })
                    .attr("x", function(d) { return x(Math.min(0, d.value)); })
                    .attr("y", function(d) { return y(d.label); })
                    .attr("width", function(d) { return Math.abs(x(d.value) - x(0)); })
                    .attr("height", y.bandwidth())

                    .on("mouseover", function(d) {
                        var xPosition = parseFloat(d3.select(this).attr("x")) + width / 2;
                        var yPosition = parseFloat(d3.select(this).attr("y")) / 2 + height / 2;
                        d3.select("#tooltip_" + svg_id)
						    .style("left", xPosition + "px")
						    .style("top", yPosition + "px")
						    .select("#value_" + svg_id)
						    .text(d.label + ":\t" + Math.round(d.value * 100) / 100)
                            .style("font-weight", "bold");
                        d3.select("#tooltip_" + svg_id).classed("hidden", false);
                    })
			        .on("mouseout", function() {
			            d3.select("#tooltip_" + svg_id).classed("hidden", true);
			        });

                svg.append("g")
                    .attr("class", "xaxis")
                    .attr("transform", "translate(0," + height + ")")
                    .call(xAxis);

                svg.append("g")
                    .attr("class", "yaxis")
                    .attr("transform", "translate(" + x(0) + ",0)")
                    .call(yAxis);

                // Title
                svg.append("text").text(title)
                    .attr("transform",
                          "translate(" + (width/2) + "," + 
                                         -20 + ")")
                    .style("text-anchor", "middle")
                    .attr("class", "title");

                // x-axis label
                svg.append("text")             
                    .attr("transform",
                          "translate(" + (width/2) + "," + 
                                         (height + margin.top) + ")")
                    .style("text-anchor", "middle")
                    .text(xlabel);

                // y-axis label
                svg.append("text")
                    .attr("transform", "rotate(-90)")
                    .attr("x", 0 - (height / 2))
                    .attr("y", 0 - margin.left)
                    .attr("dy", "1em")
                    .style("text-anchor", "middle")
                    .text(ylabel);    

            }

            function type(d) {
                d.value = +d.value;
                return d;
            }

            draw(data);
        }
        function sortBars(svg_id, data, ctrid) {
            let height = 400 - 40 - 40;
            eval('sort'+ctrid+' ^= 1;');
            var svg =  d3.select("#" + svg_id)
            var y = d3.scaleBand().rangeRound([0, height]).padding(0.3);

            if (eval('sort'+ctrid)) {
                var y0 = y.domain(data.sort(function(a, b){
                    return d3.descending(a.value, b.value);
                })
                    .map(function(d) { return d.label; }))
                    .copy();
            } else {
                var y0 = y.domain(data.sort(function(a, b){
                    return d3.ascending(a.label, b.label);
                })
                    .map(function(d) { return d.label; }))
                    .copy();
            }

            svg.selectAll(".bar")
                .sort(function(a, b) { return y0(a.label) - y0(b.label); });

            var transition = svg.transition().duration(750),
                delay = function(d, i) { return i * 50; };

            transition.selectAll(".bar")
                .delay(delay)
                .attr("y", function(d) { return y0(d.label); });

            transition.select(".yaxis")
                .call(d3.axisLeft(y)
                .tickSize(0)
                .tickPadding(6))
              .selectAll("g")
                .delay(delay);
        }
             

        function drawLine(svg_id, data, title) {
            console.log(data);

            var svg = d3.select("#" + svg_id),
            margin = {top: 20, right: 20, bottom: 110, left: 40},
            margin2 = {top: 430, right: 20, bottom: 30, left: 40},
            width = +svg.attr("width") - margin.left - margin.right,
            height = +svg.attr("height") - margin.top - margin.bottom,
            height2 = +svg.attr("height") - margin2.top - margin2.bottom;

            var parsex_label = d3.timeParse("%m/%d/%Y %H:%M");

            var x = d3.scaleTime().range([0, width]),
                x2 = d3.scaleTime().range([0, width]),
                y = d3.scaleLinear().range([height, 0]),
                y2 = d3.scaleLinear().range([height2, 0]);

            var xAxis = d3.axisBottom(x),
                xAxis2 = d3.axisBottom(x2),
                yAxis = d3.axisLeft(y);

            var brush = d3.brushX()
                .extent([[0, 0], [width, height2]])
                .on("brush end", brushed);

            var zoom = d3.zoom()
                .scaleExtent([1, Infinity])
                .translateExtent([[0, 0], [width, height]])
                .extent([[0, 0], [width, height]])
                .on("zoom", zoomed);

            var line = d3.line()
                .x(function (d) { return x(d.x_label); })
                .y(function (d) { return y(d.value); });

            var line2 = d3.line()
                .x(function (d) { return x2(d.x_label); })
                .y(function (d) { return y2(d.value); });

            var clip = svg.append("defs").append("svg:clipPath")
                .attr("id", "clip")
                .append("svg:rect")
                .attr("width", width)
                .attr("height", height)
                .attr("x", 0)
                .attr("y", 0); 


            var Line_chart = svg.append("g")
                .attr("class", "focus")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")")
                .attr("clip-path", "url(#clip)");


            var focus = svg.append("g")
                .attr("class", "focus")
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

            var context = svg.append("g")
                .attr("class", "context")
                .attr("transform", "translate(" + margin2.left + "," + margin2.top + ")");

            x.domain(d3.extent(data, function(d) { return d.x_label; }));
            y.domain([0, d3.max(data, function (d) { return d.value; })]);
            x2.domain(x.domain());
            y2.domain(y.domain());

            focus.append("g")
                .attr("class", "axis axis--x")
                .attr("transform", "translate(0," + height + ")")
                .call(xAxis);

            focus.append("g")
                .attr("class", "axis axis--y")
                .call(yAxis);

            Line_chart.append("path")
                .datum(data)
                .attr("class", "line")
                .attr("d", line);

            context.append("path")
                .datum(data)
                .attr("class", "line")
                .attr("d", line2);

            context.append("g")
                .attr("class", "axis axis--x")
                .attr("transform", "translate(0," + height2 + ")")
                .call(xAxis2);

            context.append("g")
                .attr("class", "brush")
                .call(brush)
                .call(brush.move, x.range());

            svg.append("rect")
                .attr("class", "zoom")
                .attr("width", width)
                .attr("height", height)
                .attr("transform", "translate(" + margin.left + "," + margin.top + ")")
                .call(zoom);

            function brushed() {
                if (d3.event.sourceEvent && d3.event.sourceEvent.type === "zoom") return; // ignore brush-by-zoom
                var s = d3.event.selection || x2.range();
                x.domain(s.map(x2.invert, x2));
                Line_chart.select(".line").attr("d", line);
                focus.select(".axis--x").call(xAxis);
                svg.select(".zoom").call(zoom.transform, d3.zoomIdentity
                    .scale(width / (s[1] - s[0]))
                    .translate(-s[0], 0));
            }

            function zoomed() {
                if (d3.event.sourceEvent && d3.event.sourceEvent.type === "brush") return; // ignore zoom-by-brush
                var t = d3.event.transform;
                x.domain(t.rescaleX(x2).domain());
                Line_chart.select(".line").attr("d", line);
                focus.select(".axis--x").call(xAxis);
                context.select(".brush").call(brush.move, x.range().map(t.invertX, t));
            }

            function type(d) {
                d.x_label = parsex_label(d.x_label);
                d.value = +d.value;
                return d;
            }
        }
        Sys.Application.add_load(updateChart);  // Load chart on start
    </script>
</div>
<asp:Literal ID="lblJavascript" runat="server"></asp:Literal>
