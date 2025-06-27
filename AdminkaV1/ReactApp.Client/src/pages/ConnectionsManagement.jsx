import { useCallback, useMemo } from 'react';
import { useCrudTable, useDefaultFetchList } from '@/tools/useCrudTable';

import DebugMenu from '@/tools/DebugMenu';
import ConnectionEditForm from './ConnectionEditForm';

import { z } from 'zod';

import { useEditDialog, useDefaultFetchCreate, useDefaultFetchReplace} from '@/tools/useEditDialog'
import { useDeleteDialog, useDefaultFetchDelete } from '@/tools/useDeleteDialog'

// ConnectionsManagement component - is not a pure component. it manages the state of all nested components.
// To define which nested components should be memoized
function ConnectionsManagement() {

    const fetchList = useDefaultFetchList(`/ui/connections`)

    const { reload, renderEditDialog} = useCrudTable({
        fetchList, baseColumns 
    })

    const fetchCreate = useDefaultFetchCreate(`/ui/connections`);
    const fetchReplace = useDefaultFetchReplace((entity) => `/ui/connections/${entity.excConnectionId}`)

    const { editDialog, editAction, handleCreateButtonClick } = useEditDialog(
        {
            validationSchema: connectionValidationSchema,
            addForm: (isForNew, register, errors, dirtyFields) => <ConnectionEditForm
                    isForNew={isForNew}
                    register={register}
                    errors={errors}
                    dirtyFields={dirtyFields}
            />,
            fetchCreate,
            fetchReplace,
            createDefaultEmpty,
            cloneEntity,
            reload
        }
    )

    const fetchDelete = useDefaultFetchDelete((entity)=>`/ui/connections/${entity.excConnectionId}`)

    const { deleteDialog, deleteAction } = useDeleteDialog({
            fetchDelete,
            cloneEntity,
            reload
        }
    )

    const handleDetailsButtonClick = useCallback(() => {
        // TODO await? get id
        //var id = getId(entity);
        //fetch(`${ADMINKA_API_BASE_URL}/ui/connections/${id}`, { headers: { "Content-Type": "application/json" } });
    }, []);

    const rowActions = useMemo(() => ([
        editAction,
        deleteAction,
    ]), [editAction, deleteAction]);

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

    console.log(ConnectionsManagement.name + " render") // TODO: trace.render();
    return (
        <div>
            <DebugMenu actions=
                {[
                { name: "reload crudTable", action: () => reload() }
                ]} />
            <div className="my-4">
                {renderEditDialog(rowActions, handleDetailsButtonClick, handleCreateButtonClick, multiSelectActionsMemo)}
                {editDialog}
                {deleteDialog}
            </div>
            <br />
        </div>
    );
};


const connectionValidationSchema = z.object({
    excConnectionId: z.string()
        .max(8, "No more than 8 characters").regex(/^[0-9]+$/, {
            message: "Only letters and numbers are allowed",
        }).nonempty("Required"),
    excConnectionCode: z.string().min(4, "At least 4 characters")
        .max(8, "No more than 8 characters").regex(/^[a-zA-Z0-9]+$/, {
            message: "Only letters and numbers are allowed",
        }).nonempty("Required"),
    excConnectionName: z.string().min(4, "At least 4 characters"),
    excConnectionType: z.string().min(4, "At least 4 characters").regex(/^[a-zA-Z0-9]+$/, {
        message: "Only letters and numbers are allowed",
    }),
    excConnectionDescription: z.string().min(4, "At least 4 characters"),

    excConnectionXMeta: z.string().min(4, "At least 4 characters"),
    excConnectionString: z.string().min(4, "At least 4 characters").nonempty("Required")
});

function createDefaultEmpty() {
    return {
        excConnectionId: "",
        excConnectionCode: "",
        excConnectionName: "",
        excConnectionDescription: "",
        excConnectionXMeta: "",
        excConnectionType: "PARQUET",
        excConnectionString: "",
        excConnectionIsActive: false
    }
}

function cloneEntity(entity) {
    return {
        excConnectionId: entity.excConnectionId,
        excConnectionCode: entity.excConnectionCode, excConnectionName: entity.excConnectionName,
        excConnectionDescription: entity.excConnectionDescription, excConnectionXMeta: entity.excConnectionXMeta, excConnectionType: entity.excConnectionType,
        excConnectionString: entity.excConnectionString,
        excConnectionIsActive: entity.excConnectionIsActive
    }
}

const baseColumns =
    [
        { accessorKey: "excConnectionId", header: "excConnectionId", footer: "excConnectionId" },
        {
            // excConnectionCode, excConnectionName, excConnectionType
            accessorKey: "CodeNameType", header: (<div className="text-nowrap">Code | Name | Type</div>), footer: (<div className="text-nowrap">Code | Name | Type</div>),
            accessorFn: row => {
                return [
                    row.excConnectionCode,
                    row.excConnectionName,
                    row.excConnectionType,
                ].join(' ').toLowerCase(); // Join fields as one searchable string
            },
            cell: ({ row }) => {
                const rowData = row.original;
                const excConnectionCode = rowData.excConnectionCode; // boolean
                const excConnectionName = rowData.excConnectionName; // boolean
                const excConnectionType = rowData.excConnectionType; // boolean
                return (
                    <div>
                        <div ><span className="badge bg-dark me-2">code</span>{excConnectionCode}</div>
                        <div><span className="badge bg-dark me-2">name</span>{excConnectionName}</div>
                        <div><span className="badge bg-dark me-2">type</span>{excConnectionType}</div>
                    </div>
                );
            }
        },
        { accessorKey: "excConnectionDescription", header: "excConnectionDescription", footer: "excConnectionDescription" },
        { accessorKey: "excConnectionXMeta", header: "excConnectionXMeta", footer: "excConnectionXMeta" },
        { accessorKey: "excConnectionString", header: "excConnectionString", footer: "excConnectionString" },
        {
            accessorKey: "excConnectionIsActive", header: "Active", footer: "Active",
            cell: ({ getValue }) => {
                const value = getValue(); // boolean
                return (
                    <input
                        type="checkbox"
                        checked={value}
                        disabled
                    />
                );
            }
        }
    ];

async function fetchBulkDelete(entities, setErrorMessage) {
    const ids = entities.map(e => e.excConnectionId);
    const jsonIds = JSON.stringify(ids);
    var responce = await fetchTokenized(`/ui/connections/bulkdelete`, jsonIds, "POST");
    if (responce.ok) {
        return true;
    }
    else {
        var result = await responce.json();
        setErrorMessage(result.message);
        return false
    }
};

ConnectionsManagement.whyDidYouRender = false;


export default ConnectionsManagement;