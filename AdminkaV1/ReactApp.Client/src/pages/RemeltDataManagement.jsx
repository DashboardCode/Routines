import { useEffect, useState, useRef } from 'react';
import * as d3 from 'd3';
import Tab from 'react-bootstrap/Tab';
import Tabs from 'react-bootstrap/Tabs';
import CrudTable from './CrudTable';
import DebugMenu from './DebugMenu';

function RemeltDataManager() {
    const [remeltDataContent, setData] = useState();
    const [loading, setLoading] = useState(true);

    const [reload, setReload] = useState(0); // Changing this triggers refetch

    const brushRef = useRef();
    const selectedRangeRef = useRef();
    const svgRef = useRef();

    // Hook "do something after render" Data fetching, setting up a subscription, and manually changing the DOM, logging in React
    // components are all examples of side effects.
    // UseState here, it is accesible in function's scope.
    useEffect(() => {
        fetchData(setData, setLoading); // setRemeltData - recall the component render
    }, [reload]);

    useEffect(() => {
        if (remeltDataContent) {
            drawChart(remeltDataContent, brushRef, selectedRangeRef, svgRef);
        }
    }, [remeltDataContent]);
   
    const contents = loading 
        ? <p><em>Loading... </em></p>
        : <Tabs defaultActiveKey="chart" id="uncontrolled-tab-example" className="mb-3">
            <Tab eventKey="chart" title="Chart">
                <div><svg ref={svgRef} width="800" height="400"></svg></div>
                <button className="btn btn-primary mt-3" onClick={clearSelection}>Clear</button>
                <button className="btn btn-secondary mt-3" onClick={deleteSelection}>Delete</button>
            </Tab>
            <Tab eventKey="table" title="Table">
                <CrudTable data={remeltDataContent}
                    setData={setData}
                    loading={loading}
                    addNewRow={'http://..'}
                    deleteRow={'http://..'}
                    updateRow={'http://..'}
                    showDetails={'http://..'}
                />
            </Tab>
        </Tabs>;

    return (
        <div>
            {
                console.log("render")
            }
            <DebugMenu actions=
                {[
                    { name: "refreshData", action: () => setReload((prev) => prev + 1) },
                    { name: "clearSvg", action: () => d3.select(svgRef.current).selectAll('*').remove() }
                ]} />
            <h1>Remelt Chart</h1>
            <p>This component demonstrates fetching data from the server.</p>
            <div>
                {contents}
            </div>
            <br/>
        </div>
    );
    function deleteSelection() {
        if (selectedRangeRef.current) {
            const [x0, x1] = selectedRangeRef.current;
            const filteredData = remeltDataContent.filter(d => {
                const date = new Date(d.dateTime);
                return date < x0 || date > x1;
            });
            setData(filteredData);
            clearSelection();
        }
    }
    function clearSelection() {
        d3.select(svgRef.current).select('.brush').call(brushRef.current.move, null);
        selectedRangeRef.current = null;
    }
}

async function fetchData(setRemeltData, setLoading) {
    /*
    console.log('test populateData - request data from server');
    const data = [];
    const startDateTime = new Date();

    for (let i = 0; i < 60; i++) {
        const dateTime = new Date(startDateTime.getTime() + i * 60000);
        data.push({
            dateTime: dateTime.toISOString(),
            temperatureC: 1500 + Math.floor(Math.random() * 100),
            voltage: 220 + Math.floor(Math.random() * 20)
        });
    }
    console.log('wait 5 sec');
    await new Promise(resolve => setTimeout(resolve, 3000)); // 3 sec
    console.log('data get');
    setRemeltData(data); // change the state
    */
    setLoading(true);
    const response = await fetch('remeltdata');
    if (response.ok) {
        const data = await response.json();
        setRemeltData(data);
    }
    setLoading(false);
}

function drawChart(remeltDataContent, brushRef, selectedRangeRef, svgRef) {
    var svg = svgRef.current;
    var svgSelection = d3.select(svg);
    svgSelection.selectAll('*').remove();

    const margin = { top: 20, right: 30, bottom: 30, left: 40 };
    const width = +svgSelection.attr('width') - margin.left - margin.right;
    const height = +svgSelection.attr('height') - margin.top - margin.bottom;
    const g = svgSelection.append('g').attr('transform', `translate(${margin.left},${margin.top})`);

    const x = d3.scaleTime()
        .domain(d3.extent(remeltDataContent, d => new Date(d.dateTime)))
        .range([0, width]);

    const y = d3.scaleLinear()
        .domain([d3.min(remeltDataContent, d => d.temperatureC), d3.max(remeltDataContent, d => d.temperatureC)])
        .range([height, 0]);

    const line = d3.line()
        .x(d => x(new Date(d.dateTime)))
        .y(d => y(d.temperatureC));

    g.append('g')
        .attr('transform', `translate(0,${height})`)
        .call(d3.axisBottom(x));

    g.append('g')
        .call(d3.axisLeft(y));

    g.append('path')
        .datum(remeltDataContent)
        .attr('fill', 'none')
        .attr('stroke', 'steelblue')
        .attr('stroke-width', 1.5)
        .attr('d', line);

    // Add brushing
    const brush = d3.brushX()
        .extent([[0, 0], [width, height]])
        .on('end', brushEnded);

    brushRef.current = brush;

    g.append('g')
        .attr('class', 'brush')
        .call(brush);
    function brushEnded(event) {
        if (!event.selection) return;
        const [x0, x1] = event.selection.map(x.invert);
        selectedRangeRef.current = [x0, x1];
        const filteredData = remeltDataContent.filter(d => new Date(d.dateTime) >= x0 && new Date(d.dateTime) <= x1);
        console.log('Selected range data:', filteredData);
    }
}

export default RemeltDataManager;