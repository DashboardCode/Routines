import { useCallback, useMemo } from 'react';
import { useCrudTable, useDefaultFetchList } from '@/tools/useCrudTable';

import DebugMenu from '@/tools/DebugMenu';
import TableEditForm from './TableEditForm';

import { z } from 'zod';

import { useEditDialog, useDefaultFetchCreate, useDefaultFetchReplace } from '@/tools/useEditDialog'
import { useDeleteDialog, useDefaultFetchDelete } from '@/tools/useDeleteDialog'

import { useDeleteBulkDialog, useDefaultFetchDeleteBulk } from '@/tools/useDeleteBulkDialog'

// TablesManagement component - is not a pure component. it manages the state of all nested components.
// To define which nested components should be memoized
function TablesManagement() {

    const fetchList = useDefaultFetchList(baseUrl)

    const { renderCrudTable, reload } = useCrudTable({ fetchList, baseColumns })

    const fetchCreate = useDefaultFetchCreate(baseUrl);
    const fetchReplace = useDefaultFetchReplace((formState) => `${baseUrl}/${formState.selected.excTableId}`)

    const { dialog: editDialog, action: editAction, handleCreateButtonClick } = useEditDialog(
        {
            validationSchema,
            addForm: (formState) => <TableEditForm formState={formState} />,
            fetchCreate,
            fetchReplace,
            createDefaultEmpty,
            transformSelected: cloneEntity,
            reload
        }
    )

    const fetchDelete = useDefaultFetchDelete((selected) => `${baseUrl}/${selected.excTableId}`)

    const { dialog: deleteDialog, action: deleteAction } = useDeleteDialog({
        fetchDelete,
        adoptSelected: ({ excTableId }) => ({ excTableId }),
        reload
    }
    )

    const handleDetailsButtonClick = useCallback(() => {
        // TODO await? get id
        //var id = getId(entity);
        //fetch(`${ADMINKA_API_BASE_URL}/ui/tables/${id}`, { headers: { "Content-Type": "application/json" } });
    }, []);

    const rowActions = useMemo(() => ([
        editAction,
        deleteAction,
    ]), [editAction, deleteAction]);

    // -------------------------------------------------------------------------------------

    const fetchDeleteBulk = useDefaultFetchDeleteBulk((formState) => `${baseUrl}/${formState.selected.excTableId}`)

    const { dialog: deleteDialogBulk, action: deleteBulkAction } = useDeleteBulkDialog({
        fetchDelete: fetchDeleteBulk,
        adoptSelected: (array) => array.map(({ excTableId }) => ({ excTableId })),
        reload
    });

    const multiSelectActions = useMemo(() => {
        var array = [];
        array.push(deleteBulkAction);
        return array;
    }, [deleteBulkAction]
    );

    console.log(TablesManagement.name + " render") // TODO: trace.render();
    return (
        <div>
            <DebugMenu actions=
                {[
                    { name: "reload crudTable", action: () => reload() }
                ]} />
            <div className="my-4">
                {/*<CrudTable*/}
                {/*    list={list}*/}
                {/*    errorMessage={errorMessageList}*/}
                {/*    isLoading={isLoadingList }*/}
                {/*    baseColumns={baseColumns}*/}
                {/*    handleCreateButtonClick={handleCreateButtonClick}*/}
                {/*    handleDetailsButtonClick={handleDetailsButtonClick}*/}
                {/*    rowActions={rowActions}*/}
                {/*    multiSelectActions={multiSelectActions}*/}
                {/*/>*/}
                {renderCrudTable({ rowActions, handleDetailsButtonClick, handleCreateButtonClick, multiSelectActions })}
                {editDialog}
                {deleteDialog}
                {deleteDialogBulk}
            </div>
            <br />
        </div>
    );
};

const baseUrl = "/ui/exctables";

const validationSchema = z.object({
    excTableId: z.string()
        .max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
            message: "Only letters and numbers are allowed",
        }).nonempty("Required"),

    excTableDescription: z.string().min(4, "At least 4 characters"),
    excTableXMeta: z.string().min(4, "At least 4 characters"),
    excTablePath: z.string().min(4, "At least 4 characters"),
    excTableFields: z.string().min(4, "At least 4 characters"),

    excConnectionId: z.string().max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
        message: "Only letters and numbers are allowed",
    }).nonempty("Required"),
});

let createDefaultEmpty = () => {
    return {
        excTableId: "",
        excTableDescription: "",
        excTableXMeta: "",
        excTablePath: "",
        excTableFields: "",
        excConnectionId: "1"
    }
}

let cloneEntity = (entity) => {
    return {
        excTableId: entity.excTableId,
        excTableDescription: entity.excTableDescription,
        excTableXMeta: entity.excTableXMeta,
        excTablePath: entity.excTablePath,
        excTableFields: entity.excTableFields,
        excConnectionId: entity.excConnectionId
    }
}

const baseColumns =
    [
        { accessorKey: "excTableId", header: "excTableId", footer: "excTableId" },
        { accessorKey: "excTableDescription", header: "excTableDescription", footer: "excTableDescription" },
        { accessorKey: "excTableXMeta", header: "excTableXMeta", footer: "excTableXMeta" },
        { accessorKey: "excTablePath", header: "excTablePath", footer: "excTablePath" },
        { accessorKey: "excTableFields", header: "excTableFields", footer: "excTableFields" },
        { accessorKey: "excConnectionId", header: "excConnectionId", footer: "excConnectionId" }
    ];

export default TablesManagement;
