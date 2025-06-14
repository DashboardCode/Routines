import { useMemo, useState } from 'react';
import {
    useReactTable, getCoreRowModel, getSortedRowModel, flexRender,
    getPaginationRowModel, getFilteredRowModel
} from '@tanstack/react-table';
import PropTypes from "prop-types";
import './CrudTable.css';
import DebugMenu from './DebugMenu';
import IndeterminateCheckbox from './IndeterminateCheckbox';
import * as Switch from "@radix-ui/react-switch";

const CrudTable = ({ list, 
    setList, isLoading,
    baseColumns,
    setChoosedEntity,
    multiSelectActions,
    buttonHandlers

}) => {

    const {
        handleCreateButtonClick,
        handleUpdateButtonClick,
        handleDeleteButtonClick,
        handleDetailsButtonClick
    } = buttonHandlers;

    // UseMemo is for caching constants and "non side effects calculations"
    const [isSelectable, setIsSelectable] = useState(false);
    const [rowSelection, setRowSelection] = useState({})
    const [globalFilter, setGlobalFilter] = useState('');
    

    const toggleSelectable = () => {
        setIsSelectable(!isSelectable);
        tableInstance.resetRowSelection();
        // TODO: pass table.getState().rowSelection to the dialog
    };
    
    const columns = useMemo(() => {
        var copy = [...baseColumns];
        if (isSelectable) {
            copy.unshift({
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
            copy.unshift({
                id: "actions",
                header: () => (<button className="btn btn-sm" onClick={() => { setChoosedEntity(null); handleCreateButtonClick(); }} style={{ background: "green", color: "white" }}>
                    <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">add</span>
                </button>),
                footer: () => (<button className="btn btn-sm" onClick={() => { setChoosedEntity(null); handleCreateButtonClick(); }} style={{ background: "green", color: "white" }}>
                    <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">add</span>
                </button>),
                cell: ({ row }) =>
                (<div style={{ display: "flex", gap: "5px" }}>
                    <button className="btn btn-sm" onClick={() => { setChoosedEntity(row.original); handleUpdateButtonClick(row.original); }} style={{ color: "blue" }}>
                        <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">edit_document</span>
                    </button>
                    <button className="btn btn-sm" onClick={() => { setChoosedEntity(row.original); handleDeleteButtonClick(row.original); }} style={{ color: "red" }}>
                        <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">delete_forever</span>
                    </button>
                    
                    {handleDetailsButtonClick && <button className="btn btn-sm" onClick={() => { setChoosedEntity(row.original); handleDetailsButtonClick(row.original); } } style={{ color: "gray" }}>
                        Details
                    </button>}
                </div>)
            });
        }
        return copy;
    }, [setChoosedEntity, handleCreateButtonClick, handleUpdateButtonClick, handleDeleteButtonClick, handleDetailsButtonClick, isSelectable, baseColumns]
    );

    const tableInstance = useReactTable(
        {
            data: list,
            columns: columns,
            state: {
                rowSelection, globalFilter
            },
            enableRowSelection: true, //enable row selection for all rows
            onRowSelectionChange: setRowSelection,
            getCoreRowModel: getCoreRowModel(),
            getFilteredRowModel: getFilteredRowModel(),
            getSortedRowModel: getSortedRowModel(),
            getPaginationRowModel: getPaginationRowModel(),
            globalFilterFn: 'includesString',
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

    
    multiSelectActions = 1
    const contents = isLoading
        ? <p><em>Loading... </em></p>
        : <div className="crud-panel">
            <DebugMenu actions={[
                { name: "Remove First Row", action: () => setList((l) => l.slice(1)) },
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
            {/* Button to toggle selectable mode multiSelectActions && */}
            
            <div className={`d-flex ${multiSelectActions ? 'justify-content-between' : 'justify-content-end'} align-items-center gap-2`}>
                {multiSelectActions &&
                    <Switch.Root
                        className="adminka-switch"
                        checked={isSelectable}
                        onCheckedChange={setIsSelectable}
                    >
                        <Switch.Thumb
                            className="adminka-thumb"
                        />
                    </Switch.Root>

                    //<button className="btn btn-light" onClick={toggleSelectable} style={{ marginBottom: "10px" }}>
                    //{isSelectable ? (<span style={{ fontSize: '150%', verticalAlign: 'middle', }} className="material-symbols-outlined">chevron_left</span>) : (<div><span style={{ fontSize: '150%', verticalAlign: 'middle', }} className="material-symbols-outlined">done_all</span></div>)}
                    //</button>
                }
                <input
                    type="text"
                    placeholder="Search..."
                    value={globalFilter}
                    onChange={(e) => setGlobalFilter(e.target.value)}
                    className="border p-1 mb-2"
                />
            </div>

            {tableInstance.getRowModel().rows.length > 0 ? (
                <div>
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
                    <Pagination tableInstance={tableInstance} />
                </div>
            ) : (
                    tableInstance.getPreFilteredRowModel().rows.length === 0 ? 
                        (<div className="alert alert-secondary">Empty: <button className="btn btn-sm" onClick={() => { setChoosedEntity(null); handleCreateButtonClick(true) }} style={{ background: "green", color: "white" }}>
                            Add New</button></div>) :
                        (<div className="alert alert-secondary">No results match your search.</div>)
                 )   
        
            }
        </div>;
    return contents;
}

function Pagination({ tableInstance }) {
    return (
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
                style={{ fontSize: 0.6 + 'rem' }}
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
    );
}

Pagination.propTypes = {
    tableInstance: PropTypes.object
};

export default CrudTable;