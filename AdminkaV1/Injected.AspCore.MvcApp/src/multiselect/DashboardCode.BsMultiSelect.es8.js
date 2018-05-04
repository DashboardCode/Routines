import $ from 'jquery'
import Popper from 'popper.js'

// TODO: 
// 1) setup form sended field
// 2) menu item aligment
// 3) in "using bs dropdwon" mode firs clicks doesn't work

// IIFE to declare private members
const BsMultiSelect = (($, Popper) => {
    const JQUERY_NO_CONFLICT = $.fn[pluginName];
    const pluginName = "dashboardCodeBsMultiSelect";
    const dataKey = "plugin_" + pluginName;
    const defaults = {
        items: [],
        defaults: [],
        case_sensitive: false,
        containerClass: "dashboardcode-bs-multiselect", 
        dropDownMenuClass: "dropdown-menu pl-2",
        deleteBadgeButtonStyle: { "font-size": "inherit" },
        usePopper: true,
        filterInputClass: "border-0",
        filterInputStyle: { "outline": "none", "width": "2ch" },
        selectedPanelClass: "form-control d-flex flex-row align-self",
        selectedPanelStyle: { "flex-wrap": "wrap", "min-height": "calc(2.25rem + 2px)" },
        filterInputItemClass: "badge pl-0 pt-1",
        removeSelectedItemButtonStyle: { "font-size": "100%" },
        removeSelectedItemButtonClass: "close",
        selectedItemClass: "badge pl-0 pt-1"
    };

    class Plugin {
        constructor(element, options) {
            if (typeof Popper === 'undefined') {
                throw new TypeError('DashboardCode bsMultiSelect require Popper.js (https://popper.js.org)')
            }

            this.element = element;
            this.options = $.extend({}, defaults, options);
            this.input = element;
            this.container = null;
            this.dropDownMenu = null;
            this.selectedPanel = null;
            this.filterInput = null;
            this.filterInputItem = null;
            this.popper = null;
            this.init();
        }

        createDropDown() {
            if (this.options.usePopper) {
                this.popper = new Popper(this.filterInput, this.dropDownMenu, {
                    placement: 'bottom-start',
                    modifiers: {
                        flip: {
                            behavior: ['left', 'right']
                        },
                    },
                });
            } else {
                $(this.dropDownMenu).addClass("dropdown dropdown-menu")
                $(this.dropDownMenu).data("","");
                $(this.dropDownMenu).dropdown({
                    placement: 'bottom-start',
                    flip: false,
                    reference: this.filterInput
                });
            }
            this.updateDropDown();
        }

        updateDropDown() {
            if (this.options.usePopper) {
                this.popper.update();
            } else {
                $(this.dropDownMenu).dropdown('update');
            }
        }

        hideDropDown() {
            if (this.options.usePopper) {
                console.log("popper remove show");
                $(this.dropDownMenu).removeClass('show')
            } else {
                if ($(this.dropDownMenu).hasClass('show'))
                    $(this.dropDownMenu).dropdown('toggle');
            }
        }

        showDropDown() {
            this.updateDropDown();
            if (this.options.usePopper) {
                console.log("popper add show");
                $(this.dropDownMenu).addClass('show')
            } else {
                if (!$(this.dropDownMenu).hasClass('show'))
                    $(this.dropDownMenu).dropdown('toggle');
            }
        }

        // Public methods
        removeItem(optionId) {
            $(this.selectedPanel).find('li[data-option-id="' + optionId + '"]').remove();
            $(this.dropDownMenu).find('input[id="' + optionId + '"]').prop('checked', false);
            this.updateDropDown();
        }

        getChecked() {
            var items = [];
            $(this.dropDownMenu).find('input[type="checkbox"]').each((index, checkBox) => {
                var $checkbox = $(checkBox);
                var checkBoxId = $checkbox.attr('id');
                if ($checkbox.prop('checked')) {
                   items.push(checkBoxId/*.split('-')[2]*/);
                }
            });
            console.log(items);
            return items;
        }

        close(event) {
            var $container = $(this.container);
            if (!$container.is(event.target) && $container.has(event.target).length === 0) {
                console.log("mouseup ok");
                var success = this.analyzeInputText();
                if (!success) {
                    this.filterInput.value = '';
                    this.resetDropDownMenu();
                }
                
                this.hideDropDown();
            }
        }

        find(event) {
            this.resetDropDownMenu();
        }

        resetDropDownMenu() {
            var text = this.filterInput.value.trim();
            $(this.dropDownMenu).find('li').each(function () {
                var $item = $(this);
                if (text == "") {
                    $item.show();
                }
                else {
                    var itemText = $item.text();
                    var $checkbox = $item.find('input[type="checkbox"]');
                    if (!$checkbox.prop('checked') && itemText.toLowerCase().includes(text.toLowerCase())) {
                        $item.show();
                    } else {
                        $item.hide();
                    }
                }
            });
            this.updateDropDown()
        }

        clickDropDownItem(event) {
            console.log("filter & stopPropagation");
            event.stopPropagation();
            event.preventDefault();

            var $item = $(event.currentTarget);
            var $checkbox = $item.find('input[type="checkbox"]');
            var checkBoxId = $checkbox.attr('id');
            
            if ($checkbox.prop('checked')) {
                this.deselectDownItem($checkbox, checkBoxId);
            } else {
                this.selectDownItem($item, $checkbox, checkBoxId);
            }
        }

        deselectDownItem($checkbox, checkBoxId) {
            $(this.selectedPanel).find(`li[data-option-id="${checkBoxId}"]`).remove();
            $checkbox.prop('checked', false);
        }

        selectDownItem($item, $checkbox, checkBoxId) {
            var itemText = $item.find('label').html();
            this.appendToSelectedItems(checkBoxId, itemText);
            $checkbox.prop('checked', true);
        }
        // clickDropDownItem

        appendDropDownItem(itemValue, itemText, isChecked) {
            var checkBoxId = 'item-value-' + itemValue;
            var checked = isChecked ? "checked" : "";
            var dropDownItem = $(
                `<li data-option-id="${checkBoxId}" class="dropdown-item custom-control custom-checkbox">
                        <input type="checkbox" class="custom-control-input" id="${checkBoxId}" ${checked}>
                        <label class="custom-control-label" for="${checkBoxId}">${itemText}</label>
                 </li>`).appendTo($(this.dropDownMenu)); 
            if (isChecked) {
                this.appendToSelectedItems(checkBoxId, itemText);
            }
        }

        appendToSelectedItems(checkBoxId, itemText) {
            var $item = $(`<li data-option-id="${checkBoxId}">${itemText}</li>`)
                .addClass(this.options.selectedItemClass) 
                .insertBefore($(this.filterInputItem));
            var $buttom = $("<button aria-label='Close' type='button'><span aria-hidden='true'>&times;</span></button>")
                .css(this.options.removeSelectedItemButtonStyle)
                .addClass(this.options.removeSelectedItemButtonClass)
                .appendTo($item); 
            $buttom.click((event) => { this.removeItem($(event.currentTarget).parent().data('option-id')); });
        }

        adoptFilterInputLength() {
            this.filterInput.style.width = this.filterInput.value.length + 2 + "ch";
        }

        analyzeInputText() {
            var text = this.filterInput.value.trim().toLowerCase();
            var item = [...this.dropDownMenu.querySelectorAll("LI")]
                .find(i => i.textContent.trim().toLowerCase() == text);
            if (item != undefined) {
                var $item = $(item);
                var $checkbox = $item.find('input[type="checkbox"]');
                if (!$checkbox.prop('checked')) {
                    var checkBoxId = $checkbox.attr('id');
                    this.selectDownItem($item, $checkbox, checkBoxId);
                }
                this.filterInput.value = '';
                this.resetDropDownMenu();
                return true;
            }
            return false;
        }

        init() {
            var $input = $(this.input);
            $input.hide();

            var $container = $("<div/>")
                .addClass(this.options.containerClass)
                .insertAfter($input);
                
            this.container = $container.get(0);

            var $selectedPanel = $("<ul/>")
                .addClass(this.options.selectedPanelClass)
                .css(this.options.selectedPanelStyle)
                .appendTo($container);
            this.selectedPanel = $selectedPanel.get(0);

            var $filterInputItem = $('<li/>')
                .addClass(this.options.filterInputItemClass)
                .appendTo($selectedPanel);
            this.filterInputItem = $filterInputItem.get(0)

            var $filterInput = $('<input autocomplete="off" type="text">')
                .css(this.options.filterInputStyle)
                .addClass(this.options.filterInputClass)
                .appendTo($filterInputItem);
            this.filterInput = $filterInput.get(0)

            var $dropDownMenu = $("<div/>")
                .appendTo($container);
            this.dropDownMenu = $dropDownMenu.get(0);
            $dropDownMenu.addClass(this.options.dropDownMenuClass);
            this.createDropDown();

            if (this.options.items == null) {
                this.options.items.forEach(item => {
                    var itemValue = item['value'];
                    var itemText = item['text'];
                    var isChecked = item['isChecked'];
                    this.appendDropDownItem(itemValue, itemText, isChecked);
                });
            } else {
                this.options.items = $input.find('option').each(
                    (index, option) => {
                        var itemValue = option.value;
                        var itemText = option.text;
                        var isChecked = option.selected;
                        this.appendDropDownItem(itemValue, itemText, isChecked);
                    }
                );
            }

            $dropDownMenu.click(event => {
                console.log('dropDownMenu click - stopPropagation')
                event.stopPropagation();
            });

            $(document).mouseup((event) => {
                console.log('document mouseup')
                this.close(event);
            });


            $dropDownMenu.find('li').click((event) => {
                this.clickDropDownItem(event);
            });

            $selectedPanel.click((event) => {
                console.log('selectedPanel click ' + event.target.nodeName);
                $(this.selectedPanel).find('input').val('').focus();
                //$(this.selectedPanel).find('input').val('').focus();
                if ( !(event.target.nodeName == "BUTTON" || (event.target.nodeName == "SPAN" && event.target.parentElement.nodeName == "BUTTON")))
                    this.showDropDown();
            });

            $filterInput.click((event) => {
                console.log('filterInput click')
                this.showDropDown();
            });

            $filterInput.on("keydown", (event) => {
                if (event.which == 13 || event.keyCode == 13 || event.which == 9 || event.keyCode == 9) {
                    event.preventDefault();
                } else if (event.which == 8) {
                    this.backspaceAtStartPoint = (this.filterInput.selectionStart == 0 && this.filterInput.selectionEnd == 0);
                }
            });

            $filterInput.on("keyup", (event) => {
                if (event.which == 13 || event.keyCode == 13 || event.which == 9 || event.keyCode == 9) {
                    this.analyzeInputText();
                } else if (event.which == 8 ) {
                    var startPosition = this.filterInput.selectionStart;
                    var endPosition = this.filterInput.selectionEnd;
                    if (endPosition == 0 && startPosition == 0 && this.backspaceAtStartPoint) {
                        var array = [...this.selectedPanel.querySelectorAll("LI")];
                        if (array.length >= 2) {
                            var itemToDelete = array[array.length - 2];
                            var $itemToDelete = $(itemToDelete);
                            var optionId = $itemToDelete.data("option-id");
                            var item = [...this.dropDownMenu.querySelectorAll("LI")]
                                .find(i => i.dataset.optionId == optionId);
                            var $item = $(item);
                            var $checkbox = $item.find('input[type="checkbox"]');
                            var checkBoxId = $checkbox.attr('id');
                            this.deselectDownItem($checkbox, checkBoxId);
                        }
                    }
                }
            });

            // Set on change for filter input
            $filterInput.on('input', (event) => { // keyup focus
                console.log('input');
                this.adoptFilterInputLength();
                this.find(event);
            });
        }
    }

    var jQueryInterface = function (options) {
        return this.each(function () {
            let data = $(this).data(dataKey)

            if (!data) {
                if (/dispose|hide/.test(options)) {
                    return;
                }
                else {
                    const optionsObject = (typeof options === 'object')? options:null;
                    data = new Plugin(this, optionsObject);
                    $(this).data(dataKey, data);
                }
            }

            if (typeof options === 'string') {
                var methodName = options;
                if (typeof data[methodName] === 'undefined') {
                    throw new TypeError(`No method named "${methodName}"`)
                }
                data[methodName]()
            }
        })
    }

    $.fn[pluginName] = jQueryInterface;

    // in case of mulitple $(this) it will return 1st element plugin instance
    $.fn[pluginName.charAt(0).toUpperCase() + pluginName.slice(1)] = function (options) {
        return $(this).data("plugin_" + pluginName);
    };

    $.fn[pluginName].Constructor = Plugin;

    $.fn[pluginName].noConflict = function () {
        $.fn[pluginName] = JQUERY_NO_CONFLICT
        return jQueryInterface;
    }
    return Plugin;
})($, Popper);

export default BsMultiSelect;