import React, {  useMemo, useState, useEffect, useRef} from 'react';
import {
    useReactTable, getCoreRowModel, getSortedRowModel, flexRender,
    getPaginationRowModel, getFilteredRowModel
} from '@tanstack/react-table';
import PropTypes from "prop-types";
import './CrudTable.css';
import DebugMenu from './DebugMenu';
import Pagination from './Pagination';
import IndeterminateCheckbox from './IndeterminateCheckbox';

import Button from 'react-bootstrap/Button';
import ButtonGroup from 'react-bootstrap/ButtonGroup';
import Dropdown from 'react-bootstrap/Dropdown';
import Form from 'react-bootstrap/Form';

const CrudTable = React.memo(({
    list,
    errorMessage,
    isPending,
    baseColumns,
    handleCreateButtonClick,
    handleDetailsButtonClick,
    rowActions,
    multiSelectActions/*,
    parentTrigger*/
}) => {
    //console.log('Parent / Nested (render triggers):', parentTrigger, Math.random());
    const cornerButton = useMemo(() => (<button className="btn  btn-sm btn-outline-primary" onClick={() => { handleCreateButtonClick(); }} >
        <span style={{ verticalAlign: 'middle', }} className="material-symbols-outlined">add</span>
    </button>), [handleCreateButtonClick]);

    var hasRowOrHeaderActions = (handleCreateButtonClick != null || (Array.isArray(rowActions) && rowActions.length > 0) || handleDetailsButtonClick!=null );
    
    const [rowSelection, setRowSelection] = useState({})
    const [globalFilter, setGlobalFilter] = useState('');

    const [isMultiSelectEnabled, setIsMultiSelectEnabled] = useState(false);
    const columns = useMemo(() => {
        var copy = [...baseColumns];
        if (isMultiSelectEnabled) {
            copy.unshift({
                id: "select",
                header: ({ table }) => (<span className="crud-table-selected-total">

                    <IndeterminateCheckbox
                        {...{
                            checked: table.getIsAllRowsSelected(),
                            indeterminate: table.getIsSomeRowsSelected(),
                            onChange: table.getToggleAllRowsSelectedHandler(), //it is referentially stable
                            // with pagination and filter from https://tanstack.com/table/v8/docs/framework/react/examples/row-selection
                        }}
                    />

                    </span>),
                footer: ({ table }) => <IndeterminateCheckbox
                    {...{
                        checked: table.getIsAllRowsSelected(),
                        indeterminate: table.getIsSomeRowsSelected(),
                        onChange: table.getToggleAllRowsSelectedHandler(),
                    }}
                />,
                cell: ({ row }) => (
                    <Form.Check
                        type="checkbox"
                        checked={row.getIsSelected()}
                        onChange={row.getToggleSelectedHandler()}
                    />
                ),
            });
        }
        else
        {
            if (hasRowOrHeaderActions) {
                copy.unshift({
                    id: "actions",
                    header: () => cornerButton,
                    footer: () => cornerButton,
                    cell: ({ row }) => {
                        if (rowActions?.length > 0) { 
                        var headerActions = null;
                        var firstAction = rowActions[0];
                            if (rowActions.length > 0) {
                                if (rowActions.length == 1) {
                                    headerActions = <Button type="button" variant="outline-primary" className="btn btn-sm btn-outline-primary" onClick={() => { firstAction.onClick(row.original); }}>
                                        <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">{firstAction.icon}</span>
                                    </Button>;
                                } else {
                                    headerActions = <Dropdown as={ButtonGroup}>
                                        <Button type="button" variant="outline-primary" className="btn btn-sm btn-outline-primary" onClick={() => { firstAction.onClick(row.original); }}>
                                            <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">{firstAction.icon}</span>
                                        </Button>
                                        <Dropdown.Toggle split variant="outline-primary" id="dropdown-split-basic" />

                                        <Dropdown.Menu>
                                            {rowActions.map((action, index) => (
                                                <Dropdown.Item key={index} onClick={() => { action.onClick(row.original); }}><Button type="button" variant="outline-primary" className="btn btn-sm btn-outline-primary" >
                                                    <span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">{action.icon}</span>
                                                </Button> {action.label}</Dropdown.Item>
                                            ))}
                                        </Dropdown.Menu>
                                    </Dropdown>;
                                }

                            }
                        }
                        return (<div style={{ display: "flex", gap: "5px" }}>
                            {headerActions}
                            {handleDetailsButtonClick && <button className="btn btn-primary btn-sm" onClick={() => { handleDetailsButtonClick(row.original); }} ><span style={{ verticalAlign: 'middle' }} className="material-symbols-outlined">draft</span></button>}
                        </div>)
                    }
                });
            }
        }
        return copy;
    }, [handleDetailsButtonClick, isMultiSelectEnabled, baseColumns, hasRowOrHeaderActions, rowActions, cornerButton]
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

    
    const isFirstRender = useRef(true);
    useEffect(() => {
        // this pattern used to do not cause useEffect render on first time
        if (isFirstRender.current) {
            isFirstRender.current = false;
            return; // skip execution on first mount
        }

        if (!isMultiSelectEnabled) {
            tableInstance.resetRowSelection();
        }
    }, [isMultiSelectEnabled, tableInstance]);

    const { getHeaderGroups, getFooterGroups, getRowModel } = tableInstance;

    const { pageIndex, pageSize } = tableInstance.getState().pagination;
    const pageCount = tableInstance.getPageCount();

    const selectedRowModel = tableInstance.getSelectedRowModel();

    var debugActions = React.useMemo(() => [
            {
            name: "log console table state", action: () => {
                console.info(
                    'table.options',
                    tableInstance.options
                );
                console.info(
                    'table.getState()',
                    tableInstance.getState()
                );
                console.info(
                    'selectedRowModel.flatRows',
                    selectedRowModel.flatRows
                );
            },
        },
            {
                name: "switch isMultiSelectEnabled", action: () => {
                    setIsMultiSelectEnabled(!isMultiSelectEnabled)
            },
        }], [tableInstance, selectedRowModel, isMultiSelectEnabled]);
    return isPending
        ? <p> {console.log("CrudTable render.content.isPending")} <em>Loading... </em></p>
        : <div className="crud-panel">
            <DebugMenu actions={debugActions} />
            {errorMessage && <div className="alert alert-danger" role="alert">{errorMessage}</div>}
            
            <div className={`d-flex ${multiSelectActions ? 'justify-content-between' : 'justify-content-end'} align-items-center gap-2`}>
                {multiSelectActions &&
                    <div className="d-flex align-items-center gap-1">
                        <Form.Check 
                            type="switch"
                            id="custom-switch"
                            label={
                                <div>{isMultiSelectEnabled ? <div className="badge bg-dark"><span >{Object.keys(tableInstance.getState().rowSelection).length}</span> / <span>{tableInstance.getPreFilteredRowModel().rows.length}</span> </div>:<div>Bulk!</div>}</div>
                            }
                            className="mx-2"
                            checked={isMultiSelectEnabled} onChange={(e) => setIsMultiSelectEnabled(e.target.checked)}
                        />

                        <div className={`slide-panel ${isMultiSelectEnabled ? 'show' : ''} mx-2`}>
                            <div>
                                {multiSelectActions.map(action => (
                                    <button key={action.label}
                                        className="btn btn-outline-primary btn-sm mx-1"
                                        onClick={() => action.onClick(selectedRowModel.rows)}
                                        disabled={selectedRowModel.rows.length ==  0}>
                                        <span style={{ verticalAlign: 'middle', paddingRight: '0.3rem'}} className="material-symbols-outlined">{action.icon}</span>{action.label}
                                    </button>
                                ))}
                            </div>
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
                    <Pagination tableInstance={tableInstance} pageIndex={pageIndex} pageSize={pageSize} pageCount={pageCount} />
                </div>
            ) : (
                    tableInstance.getPreFilteredRowModel().rows.length === 0 ? 
                        (<div className="alert alert-secondary">Empty: <button className="btn btn-sm" onClick={() => { handleCreateButtonClick() }} style={{ background: "green", color: "white" }}>
                            Add New</button></div>) :
                        (<div className="alert alert-secondary">No results match your search.</div>)
                 )   
        
            }
        </div>;
})

CrudTable.displayName ="CrudTable"

CrudTable.whyDidYouRender = true;

CrudTable.propTypes = {
    list: PropTypes.array,
    errorMessage: PropTypes.node,
    isPending: PropTypes.bool.isRequired,
    baseColumns: PropTypes.array.isRequired,
    multiSelectActions: PropTypes.array,
    handleCreateButtonClick: PropTypes.func,
    handleDetailsButtonClick: PropTypes.func,
    rowActions: PropTypes.array,
    parentTrigger: PropTypes.number
}

export default CrudTable;