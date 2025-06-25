import React from 'react';
import PropTypes from "prop-types";

// use tableInstance only for calling methods 
// do not relly on 
const Pagination = React.memo(({ tableInstance, pageIndex, pageSize, pageCount }) => {
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
                onClick={() => tableInstance.setPageIndex(pageCount - 1)}
                disabled={!tableInstance.getCanNextPage()}
            >
                {'>>'}
            </button>
            <div>Page</div>
            <strong>
                {pageIndex + 1} of {pageCount}
            </strong>
            | Go to page:
            <input
                type="number"
                min="1"
                max={pageCount}
                defaultValue={pageIndex + 1}
                onChange={e => {
                    const page = e.target.value ? Number(e.target.value) - 1 : 0
                    tableInstance.setPageIndex(page)
                }}
                className="border p-1 rounded w-16"
            />
            <select
                value={pageSize}
                onChange={e => {
                    tableInstance.setPageSize(Number(e.target.value))
                }}
            >
                {[10, 20, 30, 40, 50].map(pageSize2 => (
                    <option key={pageSize2} value={pageSize2}>
                        Show {pageSize2}
                    </option>
                ))}
            </select>
        </div>
    );
})

Pagination.displayName = "Pagination";

Pagination.propTypes = {
    tableInstance: PropTypes.object,
    pageIndex: PropTypes.number,
    pageSize: PropTypes.number,
    pageCount: PropTypes.number
};

export default Pagination;