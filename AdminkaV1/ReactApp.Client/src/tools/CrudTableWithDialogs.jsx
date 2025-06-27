import React, {useMemo} from 'react';

import PropTypes from "prop-types";
import CrudTable from './CrudTable';


const CrudTableWithDialogs = React.memo(({
    baseColumns,
    list,
    errorMessageList,
    isLoadingList,
    rowActions,
    handleCreateButtonClick,
    handleDetailsButtonClick
}) => {
  
    const multiSelectActionsMemo = useMemo(() => {
        var multiSelectActions = null;
        var isMultiSelectEdit = true;
        var isMultiSelectDelete = true;
        if (isMultiSelectEdit) {
            if (multiSelectActions == null)
                multiSelectActions = [];
            multiSelectActions.push({ handleButtonClick: () => { handleCreateButtonClick() /*TEST*/ }, buttonTitle: "Edit" });
        }
        if (isMultiSelectDelete) {
            if (multiSelectActions == null)
                multiSelectActions = [];
            multiSelectActions.push({ handleButtonClick: () => { handleCreateButtonClick() /*TEST*/ }, buttonTitle: "Delete" });
        }
        return multiSelectActions;
    }, [handleCreateButtonClick]
    );

    console.log("CrudTableWithDialogs render")
    return (
        <div>
            <CrudTable
                list={list}
                errorMessage={errorMessageList}
                isLoading={isLoadingList}
                baseColumns={baseColumns}
                multiSelectActions={multiSelectActionsMemo}
                handleCreateButtonClick={handleCreateButtonClick}
                handleDetailsButtonClick={handleDetailsButtonClick}
                rowActions={rowActions}
            />
            {rowActions.map((a, index) => a.createDialog(index))}
        </div>
    );
})

CrudTableWithDialogs.displayName = "CrudTableWithDialogs"; // for debugging purposes



CrudTableWithDialogs.propTypes = {
    baseColumns: PropTypes.array, 
    list: PropTypes.array,
    errorMessageList: PropTypes.node,
    isLoadingList: PropTypes.bool,

    rowActions: PropTypes.array,
    handleCreateButtonClick: PropTypes.func,
    handleDetailsButtonClick: PropTypes.func
};

CrudTableWithDialogs.whyDidYouRender = false;

export default CrudTableWithDialogs;