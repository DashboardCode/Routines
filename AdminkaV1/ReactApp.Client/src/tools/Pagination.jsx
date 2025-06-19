import PropTypes from "prop-types";
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

export default Pagination;