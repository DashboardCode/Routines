import { useMemo, useState} from 'react';
import {
    useReactTable, getCoreRowModel, getSortedRowModel, flexRender,
    getPaginationRowModel, getFilteredRowModel
} from '@tanstack/react-table';
import PropTypes from "prop-types";
import './CrudTable.css';
import DebugMenu from './DebugMenu';
import Pagination from './Pagination';
import IndeterminateCheckbox from './IndeterminateCheckbox';
import * as Switch from "@radix-ui/react-switch";

function CrudTable({
    list,
    setList,
    reload,
    isLoading,
    baseColumns,
    options = {}
}){

    var { multiSelectActions, buttonHandlers } = options;
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
    
    //const toggleSelectable = () => {
    //    setIsSelectable(!isSelectable);
    //    tableInstance.resetRowSelection();
    //    // TODO: pass table.getState().rowSelection to the dialog
    //};
    
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
        }
        else
        {
            copy.unshift({
                id: "actions",
                header: () => (<button className="btn btn-sm" onClick={() => { handleCreateButtonClick(); }} style={{ background: "green", color: "white" }}>
                    <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">add</span>
                </button>),
                footer: () => (<button className="btn btn-sm" onClick={() => { handleCreateButtonClick(); }} style={{ background: "green", color: "white" }}>
                    <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">add</span>
                </button>),
                cell: ({ row }) =>
                (<div style={{ display: "flex", gap: "5px" }}>
                    <button className="btn btn-sm" onClick={() => { handleUpdateButtonClick(row.original); }} style={{ color: "blue" }}>
                        <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">edit_document</span>
                    </button>
                    <button className="btn btn-sm" onClick={() => { handleDeleteButtonClick(row.original); }} style={{ color: "red" }}>
                        <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">delete_forever</span>
                    </button>
                    
                    {handleDetailsButtonClick && <button className="btn btn-sm" onClick={() => { handleDetailsButtonClick(row.original); } } style={{ color: "gray" }}>
                        Details
                    </button>}
                </div>)
            });
        }
        return copy;
    }, [handleCreateButtonClick, handleUpdateButtonClick, handleDeleteButtonClick, handleDetailsButtonClick, isSelectable, baseColumns]
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

    const contents = isLoading
        ? <p><em>Loading... </em></p>
        : <div className="crud-panel">
            <DebugMenu actions={[
                { name: "refreshData", action: () => reload() },
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
                    <div className="d-flex align-items-center gap-1">
                        <span className="text-secondary">Bulk: </span>
                        <Switch.Root className="adminka-switch" checked={isSelectable} onCheckedChange={setIsSelectable}>
                            <Switch.Thumb className="adminka-thumb"/>
                        </Switch.Root>

                        <div className={`slide-panel ${isSelectable ? 'show' : ''} mx-2`}>
                            <button className="btn btn-secondary btn-sm" onClick={() => { handleCreateButtonClick(); }}  disabled={!tableInstance.getIsSomeRowsSelected()}>Edit</button>
                            <button className="btn btn-secondary btn-sm" onClick={() => { handleDeleteButtonClick(); }}  disabled={!tableInstance.getIsSomeRowsSelected()}>Delete</button>
                        </div>
                    </div>
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
                        (<div className="alert alert-secondary">Empty: <button className="btn btn-sm" onClick={() => { handleCreateButtonClick() }} style={{ background: "green", color: "white" }}>
                            Add New</button></div>) :
                        (<div className="alert alert-secondary">No results match your search.</div>)
                 )   
        
            }
        </div>;
    return contents;
}

CrudTable.propTypes = {
    list: PropTypes.array,
    setList: PropTypes.func,
    reload: PropTypes.func,
    isLoading: PropTypes.bool,
    baseColumns: PropTypes.array,
    options: PropTypes.shape({
        multiSelectActions: PropTypes.array,
        buttonHandlers: PropTypes.shape({
            handleCreateButtonClick: PropTypes.func/*.isRequired*/,
            handleUpdateButtonClick: PropTypes.func,
            handleDeleteButtonClick: PropTypes.func,
            handleDetailsButtonClick: PropTypes.func
        })
    })
}
export default CrudTable;