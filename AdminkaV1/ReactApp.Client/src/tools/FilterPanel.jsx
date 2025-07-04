import React, { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router-dom';
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

        const others = filters.filter((f) => f.type !== FILTER_TYPES.TEXT);
        const hasOtherFilters = others.length > 0;

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
                    placeholder="Search..."
                    aria-label="Text input with dropdown button"
                    value={inputText}
                    onChange={handleInputChange}
                />
                <Dropdown as={InputGroup.Append}>
                    <Dropdown.Toggle variant="outline-secondary">
                        More..
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
export { FILTER_TYPES, FilterTags, FilterInput }