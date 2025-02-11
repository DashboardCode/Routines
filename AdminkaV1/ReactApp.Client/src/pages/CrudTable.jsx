import React, { useMemo, useState, useRef } from 'react';
import {
    useReactTable, getCoreRowModel, getSortedRowModel, flexRender,
    getPaginationRowModel
} from '@tanstack/react-table';
import PropTypes from "prop-types";

import './CrudTable.css';
import DebugMenu from './DebugMenu';
//import {
//    getFilteredRowModel,
//    getPaginationRowModel,
//    Table
//} from '@tanstack/react-table'

const CrudTable = ({ data,
    setData, loading ,
    addNewRow, deleteRow, updateRow, showDetails
}) => {
    // UseMemo is for caching constants and "non side effects calculations"
    const [isSelectable, setIsSelectable] = useState(false);
    const [rowSelection, setRowSelection] = useState({})

    const toggleSelectable = () => {
        setIsSelectable(!isSelectable);
        tableInstance.resetRowSelection();
        // TODO: pass table.getState().rowSelection to the dialog

    };

    const columns = useMemo(() => {
        let baseColumns = 
        [
                { accessorKey: "dateTime", header: "DateTime", footer: "DateTime" },
                { accessorKey: "temp", header: "Temp. (C)", footer: "Temp. (C)" },
                { accessorKey: "voltage", header: "Voltage (V)", footer: "Voltage (V)" },
        ];

        if (isSelectable) {
            baseColumns.unshift({
                id: "select",
                header: ({ table }) => (<span className="crud-table-selected-total"><IndeterminateCheckbox
                    {...{
                        checked: table.getIsAllRowsSelected(),
                        indeterminate: table.getIsSomeRowsSelected(),
                        onChange: table.getToggleAllRowsSelectedHandler(),

                        // with page and filter from https://tanstack.com/table/v8/docs/framework/react/examples/row-selection
                        //checked: table.getIsAllPageRowsSelected(),
                        //indeterminate: table.getIsSomePageRowsSelected(),
                        //onChange: table.getToggleAllPageRowsSelectedHandler()

                    }} />{Object.keys(table.getState().rowSelection).length + "/" +
                        table.getPreFilteredRowModel().rows.length    
                    }</span>),
                footer: ({ table }) => (<span className="crud-table-selected-total"><IndeterminateCheckbox
                    {...{
                        checked: table.getIsAllRowsSelected(),
                        indeterminate: table.getIsSomeRowsSelected(),
                        onChange: table.getToggleAllRowsSelectedHandler()

                        // with page and filter from https://tanstack.com/table/v8/docs/framework/react/examples/row-selection
                        //checked: table.getIsAllPageRowsSelected(),
                        //indeterminate: table.getIsSomePageRowsSelected(),
                        //onChange: table.getToggleAllPageRowsSelectedHandler()

                    }} />{Object.keys(table.getState().rowSelection).length + "/" +
                        table.getPreFilteredRowModel().rows.length
                    }</span>),
                cell: ({ row }) => (
                        <IndeterminateCheckbox
                            {...{
                                checked: row.getIsSelected(),
                                disabled: !row.getCanSelect(),
                                indeterminate: row.getIsSomeSelected(),
                                onChange: row.getToggleSelectedHandler(),
                            }}
                        />
                ),
            });
        } else
        {
            baseColumns.unshift({
                id: "actions",
                header: () => (<button onClick={() => addNewRow(data, setData)} style={{ background: "green", color: "white" }}>
                        Add New
                </button>),
                footer: () => (<button onClick={() => addNewRow(data, setData)} style={{ background: "green", color: "white" }}>
                    Add New
                </button>),
                cell: ({ row }) => (<div style={{ display: "flex", gap: "5px" }}>
                        <button onClick={() => updateRow(data, setData, row.original.id)} style={{ color: "blue" }}>
                            Edit
                        </button>
                        <button onClick={() => deleteRow(data, setData, row.original.id)} style={{ color: "red" }}>
                            Delete
                        </button>
                        <button onClick={() => showDetails(row.original)} style={{ color: "gray" }}>
                            Details
                        </button>
                    </div>)
            });
        }
        return baseColumns;
    }, [data, setData, addNewRow, updateRow, deleteRow, showDetails, isSelectable]
    );

    const tableInstance = useReactTable(
        {
            data: data,
            columns: columns,
            state: {
                rowSelection,
            },
            enableRowSelection: true, //enable row selection for all rows
            onRowSelectionChange: setRowSelection,
            getCoreRowModel: getCoreRowModel(),
            getSortedRowModel: getSortedRowModel(),
            getPaginationRowModel: getPaginationRowModel(),
            meta: {
                toggleRowClass: (row) => {
                    if (row.getIsSelected()) {
                        return "crud-table-selected-row";
                    } else {
                        return "";
                    }
                }
            }
        }
    );
    const { getHeaderGroups, getFooterGroups, getRowModel } = tableInstance;

    const contents = loading
        ? <p><em>Loading... </em></p>
        : <div className="crud-panel">
            <DebugMenu actions={[
                { name: "Remove First Row", action: () => setData((prevData) => prevData.slice(1)) },
                {
                    name: "log console table.getSelectedRowModel().flatRows", action: () => {
                        console.info(
                            'table.options',
                            tableInstance.options
                        );
                        console.info(
                            'table.getState()',
                            tableInstance.getState()
                        );
                        console.info(
                            'table.getSelectedRowModel().flatRows',
                            tableInstance.getSelectedRowModel().flatRows
                        );
                    }
                }]} />
            {/* Button to toggle selectable mode */}
            <button onClick={toggleSelectable} style={{ marginBottom: "10px" }}>
                {isSelectable ? "Disable Selection" : "Enable Selection"}
            </button>
            <table className="table table-striped crud-table" aria-labelledby="tableLabel">
                    <thead>
                        {getHeaderGroups().map(headerGroup => (
                            <tr key={headerGroup.id}>
                                {headerGroup.headers.map(header => (<th key={header.id} colSpan={header.colSpan}>
                                        {flexRender(header.column.columnDef.header, header.getContext())}</th>))}
                            </tr>
                        ))}
                    </thead>
                    <tbody>
                        {/*{remeltDataContent.map(remeltData =>*/}
                        {/*    <tr key={remeltData.dateTime}>*/}
                        {/*        <td>{remeltData.dateTime}</td>*/}
                        {/*        <td>{remeltData.temperatureC}</td>*/}
                        {/*        <td>{remeltData.voltage}</td>*/}
                        {/*    </tr>*/}
                        {/*)}*/}
                    {getRowModel().rows.map(row => {
                        return (
                            <tr key={row.id} className={tableInstance.options.meta.toggleRowClass(row)}>
                                {row.getVisibleCells().map(cell => {
                                    return (
                                        <td key={cell.id}>
                                            {flexRender(
                                                cell.column.columnDef.cell,
                                                cell.getContext()
                                            )}
                                        </td>
                                    )
                                })}
                            </tr>
                        )
                    })}

                </tbody>
                <tfoot>
                    {getFooterGroups().map(footerGroup => (
                        <tr key={footerGroup.id}>
                            {footerGroup.headers.map(header => (<th key={header.id} colSpan={header.colSpan}>
                                {flexRender(header.column.columnDef.footer, header.getContext())}</th>))}
                        </tr>
                    ))}
                </tfoot>
                </table>

            <div className="d-flex justify-content-end align-items-center gap-2">
                <button
                    className="border rounded p-1"
                    style={{ fontSize: 0.6 + 'rem' }}
                    onClick={() => tableInstance.setPageIndex(0)}
                    disabled={!tableInstance.getCanPreviousPage()}
                >
                    {'<<'}
                </button>
                <button
                    className="border rounded p-1"
                    style={{ fontSize: 0.6 + 'rem' }}
                    onClick={() => tableInstance.previousPage()}
                    disabled={!tableInstance.getCanPreviousPage()}
                >
                    {'<'}
                </button>
                <button
                    className="border rounded p-1"
                    style={{ fontSize: 0.6 + 'rem' }}
                    onClick={() => tableInstance.nextPage()}
                    disabled={!tableInstance.getCanNextPage()}
                >
                    {'>'}
                </button>
                <button
                    className="border rounded p-1"
                    style= {{fontSize: 0.6 + 'rem'}}
                    onClick={() => tableInstance.setPageIndex(tableInstance.getPageCount() - 1)}
                    disabled={!tableInstance.getCanNextPage()}
                >
                    {'>>'}
                </button>
                    <div>Page</div>
                    <strong>
                        {tableInstance.getState().pagination.pageIndex + 1} of {tableInstance.getPageCount()}
                    </strong>
                    | Go to page:
                    <input
                        type="number"
                        min="1"
                        max={tableInstance.getPageCount()}
                        defaultValue={tableInstance.getState().pagination.pageIndex + 1}
                        onChange={e => {
                            const page = e.target.value ? Number(e.target.value) - 1 : 0
                            tableInstance.setPageIndex(page)
                        }}
                        className="border p-1 rounded w-16"
                    />
                <select
                    value={tableInstance.getState().pagination.pageSize}
                    onChange={e => {
                        tableInstance.setPageSize(Number(e.target.value))
                    }}
                >
                    {[10, 20, 30, 40, 50].map(pageSize => (
                        <option key={pageSize} value={pageSize}>
                            Show {pageSize}
                        </option>
                    ))}
                </select>
            </div>
            
            
        </div>;
    //const contents = <p><em>Loading... </em></p>;
    return contents;
}

function addNewRow(data, setData) {
    const newId = data.length ? Math.max(...data.map(d => d.id)) + 1 : 1;
    setData([...data, { id: newId, name: `New User ${newId}`, age: 20 }]);
};

function deleteRow(data, setData, id) {
    setData(data.filter((row) => row.id !== id));
};

function updateRow(data, setData, id) {
    setData(
        data.map((row) =>
            row.id === id ? { ...row, name: row.name + " (Updated)" } : row
        )
    );
};

// Show details of a row (for now, just alert the data)
function showDetails(row) {
    alert(`Details:\nID: ${row.id}\nName: ${row.name}\nAge: ${row.age}`);
};

function IndeterminateCheckbox({
    indeterminate,
    className = '',
    ...rest
}) {
    const ref = useRef(null);

    React.useEffect(() => {
        if (typeof indeterminate === 'boolean') {
            ref.current.indeterminate = !rest.checked && indeterminate
        }
    }, [ref, indeterminate, rest])

    return (
        <input
            type="checkbox"
            ref={ref}
            className={className + ' cursor-pointer'}
            {...rest}
        /> 
    )
}

IndeterminateCheckbox.propTypes = {
    indeterminate: PropTypes.bool,
    className: PropTypes.string,
    getRowSelectionLength: PropTypes.func,
    getRowTotalLength: PropTypes.func
};

export default CrudTable;