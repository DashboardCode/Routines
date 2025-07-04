import React, { useEffect, useState, useMemo } from 'react';
import { useSearchParams } from 'react-router-dom';
import {
    useReactTable,
    getCoreRowModel,
    getFilteredRowModel,
    flexRender,
} from '@tanstack/react-table';
import Button from 'react-bootstrap/Button';
import Dropdown from 'react-bootstrap/Dropdown';
import Modal from 'react-bootstrap/Modal';
import CloseButton from 'react-bootstrap/CloseButton';
import InputGroup from 'react-bootstrap/InputGroup';
import FormControl from 'react-bootstrap/FormControl';
import Select from 'react-select';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import 'bootstrap/dist/css/bootstrap.min.css';

const FILTER_TYPES = {
    VALUE: 'value',
    DATE: 'date',
    TEXT: 'text',
};

function FilterTags({ filters, removeFilter }) {
    const visibleTags = filters.filter((f) => {
        if (f.type === FILTER_TYPES.TEXT) {
            return f.value && filters.some((other) => other.type !== FILTER_TYPES.TEXT);
        }
        return true;
    });

    const sortedTags = [...visibleTags].sort((a, b) => {
        if (a.type === FILTER_TYPES.TEXT) return -1;
        if (b.type === FILTER_TYPES.TEXT) return 1;
        return 0;
    });

    return (
        <div className="d-flex flex-wrap gap-2">
            {sortedTags.map((filter) => (
                <div
                    key={filter.id}
                    className="d-flex align-items-center bg-white border px-2 py-1 rounded shadow-sm"
                >
                    <span className="me-2">
                        {filter.type === FILTER_TYPES.VALUE && `Value: ${filter.value?.label || '—'}`}
                        {filter.type === FILTER_TYPES.DATE && `Date: ${filter.value?.from?.toLocaleDateString?.() || '—'} - ${filter.value?.to?.toLocaleDateString?.() || '—'}`}
                        {filter.type === FILTER_TYPES.TEXT && `Text: ${filter.value || ''}`}
                    </span>
                    <CloseButton onClick={() => removeFilter(filter.id)} className="ms-1" />
                </div>
            ))}
        </div>
    );
}

function FilterInput({ filters, updateFilter }) {
    const [searchParams, setSearchParams] = useSearchParams();
    const [activeFilter, setActiveFilter] = useState(null);
    const [showModal, setShowModal] = useState(false);
    const [inputText, setInputText] = useState('');

    const handleInputChange = (e) => {
        const text = e.target.value;
        setInputText(text);

        if (text.trim() === '') {
            updateFilter('text', undefined);
            setSearchParams((prev) => {
                const newParams = new URLSearchParams(prev);
                newParams.delete('text');
                return newParams;
            });
            return;
        }

        updateFilter('text', text);
    };

    useEffect(() => {
        const text = searchParams.get('text') || '';
        setInputText(text);
    }, [searchParams]);

    const addFilter = (type) => {
        const newFilter = {
            id: type,
            type,
            value: type === FILTER_TYPES.VALUE
                ? null
                : type === FILTER_TYPES.DATE
                    ? { from: null, to: null }
                    : '',
        };
        setActiveFilter(newFilter);
        setShowModal(true);
    };

    const handleSave = () => {
        if (activeFilter) {
            updateFilter(activeFilter.id, activeFilter.value);
        }
        setShowModal(false);
    };

    return (
        <>
            <InputGroup style={{ maxWidth: 300 }}>
                <FormControl
                    placeholder="Filter..."
                    aria-label="Text input with dropdown button"
                    value={inputText}
                    onChange={handleInputChange}
                />
                <Dropdown as={InputGroup.Append}>
                    <Dropdown.Toggle variant="outline-secondary">
                        Add Filter
                    </Dropdown.Toggle>
                    <Dropdown.Menu>
                        <Dropdown.Item onClick={() => addFilter(FILTER_TYPES.VALUE)}>
                            Value Filter
                        </Dropdown.Item>
                        <Dropdown.Item onClick={() => addFilter(FILTER_TYPES.DATE)}>
                            Date Range Filter
                        </Dropdown.Item>
                    </Dropdown.Menu>
                </Dropdown>
            </InputGroup>

            <Modal show={showModal} onHide={() => setShowModal(false)}>
                <Modal.Header closeButton>
                    <Modal.Title>Configure Filter</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {activeFilter?.type === FILTER_TYPES.VALUE && (
                        <Select
                            options={[{ label: 'Option 1', value: '1' }, { label: 'Option 2', value: '2' }]}
                            value={activeFilter.value}
                            onChange={(val) => setActiveFilter({ ...activeFilter, value: val })}
                        />
                    )}
                    {activeFilter?.type === FILTER_TYPES.DATE && (
                        <div className="d-flex align-items-center gap-2">
                            <DatePicker
                                selected={activeFilter.value.from}
                                onChange={(date) =>
                                    setActiveFilter({
                                        ...activeFilter,
                                        value: { ...activeFilter.value, from: date },
                                    })
                                }
                                selectsStart
                                startDate={activeFilter.value.from}
                                endDate={activeFilter.value.to}
                                placeholderText="From"
                                className="form-control"
                            />
                            <span>-</span>
                            <DatePicker
                                selected={activeFilter.value.to}
                                onChange={(date) =>
                                    setActiveFilter({
                                        ...activeFilter,
                                        value: { ...activeFilter.value, to: date },
                                    })
                                }
                                selectsEnd
                                startDate={activeFilter.value.from}
                                endDate={activeFilter.value.to}
                                placeholderText="To"
                                className="form-control"
                            />
                        </div>
                    )}
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="secondary" onClick={() => setShowModal(false)}>
                        Cancel
                    </Button>
                    <Button variant="primary" onClick={handleSave}>
                        Save Filter
                    </Button>
                </Modal.Footer>
            </Modal>
        </>
    );
}

export default function FilterPage() {
    const [filters, setFilters] = useState([]);
    const [searchParams, setSearchParams] = useSearchParams();

    const data = useMemo(() => [
        { id: 1, name: 'Item A', value: '1', date: '2024-01-01' },
        { id: 2, name: 'Item B', value: '2', date: '2024-03-01' },
        { id: 3, name: 'Filter Me', value: '1', date: '2024-05-01' },
    ], []);

    const columns = useMemo(() => [
        {
            accessorKey: 'id',
            header: 'ID'
        },
        {
            accessorKey: 'name',
            header: 'Name'
        },
        {
            accessorKey: 'value',
            header: 'Value'
        },
        {
            accessorKey: 'date',
            header: 'Date'
        },
    ], []);

    const table = useReactTable({
        data,
        columns,
        state: {
            globalFilter: filters.find(f => f.type === 'text')?.value || '',
        },
        getCoreRowModel: getCoreRowModel(),
        getFilteredRowModel: getFilteredRowModel(),
        globalFilterFn: (row, columnId, filterValue) => {
            return String(row.getValue('name')).toLowerCase().includes(filterValue.toLowerCase());
        }
    });

    const rows = useMemo(() => {
        return table.getFilteredRowModel().rows.filter(row => {
            return filters.every(filter => {
                if (filter.type === 'value') {
                    return row.original.value === filter.value?.value;
                }
                if (filter.type === 'date') {
                    const d = new Date(row.original.date);
                    const from = filter.value?.from;
                    const to = filter.value?.to;
                    return (!from || d >= from) && (!to || d <= to);
                }
                return true;
            });
        });
    }, [table, filters]);

    useEffect(() => {
        const value = searchParams.get('value');
        const from = searchParams.get('from');
        const to = searchParams.get('to');
        const text = searchParams.get('text') || '';

        const newFilters = [];
        if (value) {
            newFilters.push({ id: 'value', type: FILTER_TYPES.VALUE, value: { label: value, value } });
        }
        if (from || to) {
            newFilters.push({
                id: 'date',
                type: FILTER_TYPES.DATE,
                value: {
                    from: from ? new Date(from) : null,
                    to: to ? new Date(to) : null,
                },
            });
        }
        if (text) {
            newFilters.unshift({ id: 'text', type: FILTER_TYPES.TEXT, value: text });
        }
        setFilters(newFilters);
    }, [searchParams]);

    const syncToSearchParams = (updatedFilters) => {
        const params = new URLSearchParams();
        updatedFilters.forEach((f) => {
            if (f.type === FILTER_TYPES.VALUE && f.value) {
                params.set('value', f.value.value);
            }
            if (f.type === FILTER_TYPES.DATE && f.value) {
                if (f.value.from) params.set('from', f.value.from.toISOString());
                if (f.value.to) params.set('to', f.value.to.toISOString());
            }
            if (f.type === FILTER_TYPES.TEXT && f.value) {
                params.set('text', f.value);
            }
        });
        setSearchParams(params);
    };

    const updateFilter = (id, value) => {
        let updated = filters.filter((f) => f.id !== id);
        if (value !== undefined && value !== '') {
            updated = [{ id, type: id, value }, ...updated];
        }
        setFilters(updated);
        syncToSearchParams(updated);
    };

    const removeFilter = (id) => {
        const updated = filters.filter((f) => f.id !== id);
        setFilters(updated);
        syncToSearchParams(updated);
    };

    return (
        <div className="p-4 border rounded shadow-sm bg-light">
            <div className="d-flex align-items-center gap-3 mb-3">
                <FilterTags filters={filters} removeFilter={removeFilter} />
                <FilterInput
                    filters={filters}
                    updateFilter={updateFilter}
                />
            </div>

            <table className="table table-bordered bg-white">
                <thead>
                    {table.getHeaderGroups().map((headerGroup) => (
                        <tr key={headerGroup.id}>
                            {headerGroup.headers.map((header) => (
                                <th key={header.id}>
                                    {flexRender(header.column.columnDef.header, header.getContext())}
                                </th>
                            ))}
                        </tr>
                    ))}
                </thead>
                <tbody>
                    {rows.map((row) => (
                        <tr key={row.id}>
                            {row.getVisibleCells().map((cell) => (
                                <td key={cell.id}>
                                    {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                </td>
                            ))}
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}