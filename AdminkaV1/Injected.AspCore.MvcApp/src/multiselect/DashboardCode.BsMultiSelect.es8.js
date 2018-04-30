import $ from 'jquery'
import Popper from 'popper.js'


// TODO: 1) setup form sended field
// 2) auto shrinked text input (and better user expirience)
// 3) menu item aligment
// 4) in "using bs dropdwon" mode firs clicks doesn't work

// IIFE to declare private members
const BsMultiSelect = (($, Popper) => {
    const JQUERY_NO_CONFLICT = $.fn[pluginName];
    const pluginName = "dashboardCodeBsMultiSelect";
    const dataKey = "plugin_" + pluginName;
    const defaults = {
        items: [],
        defaults: [],
        case_sensitive: false,
        containerClass: "dashboardcode-bs-multiselect", //  dropdown
        selectedPanelClass: "form-control",
        dropDownMenuClass: "dropdown-menu pl-2",
        deleteBadgeButtonStyle: { "font-size": "inherit" },
        usePopper: true
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
                $(this.dropDownMenu).removeClass('show')
            } else {
                if ($(this.dropDownMenu).hasClass('show'))
                    $(this.dropDownMenu).dropdown('toggle');
            }
        }

        showDropDown() {
            this.updateDropDown();
            if (this.options.usePopper) {
                $(this.dropDownMenu).addClass('show')
            } else {
                if (!$(this.dropDownMenu).hasClass('show'))
                    $(this.dropDownMenu).dropdown('toggle');
            }
        }

        // Public methods
        removeItem(optionId) {
            var $container = $(this.container);
            $container.find('li[data-option-id="' + optionId + '"]').remove();
            $container.find('input[id="' + optionId + '"]').prop('checked', false);
            this.updateDropDown();
        }

         getChecked() {
            var items = [];
            $(this.dropDownMenu).find('input[type="checkbox"]').each((index, checkBox) => {
                var $checkBox = $(checkBox);
                var checkBoxId = $checkBox.attr('id');
                if ($checkBox.prop('checked')) {
                   items.push(checkBoxId/*.split('-')[2]*/);
                }
            });
            console.log(items);
            return items;
        }

        close(event) {
            console.log("mouseup");
            var $container = $(this.container);
            if (!$container.is(event.target) && $container.has(event.target).length === 0) {
                console.log("mouseup ok");
                this.hideDropDown();
                this.updateDropDown();
            }
        }

        find(event) {
            var $filter = $(event.currentTarget);
            var text = $filter.val();
            
            $(this.dropDownMenu).find('.dropdown-item').each(function () {
                var item = $(this);
                var itemText = item.text();
                if (itemText.toLowerCase().includes(text.toLowerCase())) {
                    item.show();
                } else {
                    item.hide();
                }
            });
            this.updateDropDown()
        }

        filter(event) {
            console.log("filter & stopPropagation");
            event.stopPropagation();
            event.preventDefault();

            var item = $(event.currentTarget);
            var $checkbox = item.find('input[type="checkbox"]');
            var checkBoxId = $checkbox.attr('id');
            
            if ($checkbox.prop('checked')) {
                $(this.selectedPanel).find(`li[data-option-id="${checkBoxId}"]`).remove();
                $checkbox.prop('checked', false);

            } else {
                var itemText = item.find('label').html();
                this.appendToSelectedItems(checkBoxId, itemText);
                $checkbox.prop('checked', true);
            }
        }

        appendDropDownItem(itemValue, itemText, isChecked) {
            var checkBoxId = 'item-value-' + itemValue;
            var checked = isChecked ? "checked" : "";
            var dropDownItem = $(
                `<li class="dropdown-item custom-control custom-checkbox">
                        <input type="checkbox" class="custom-control-input" id="${checkBoxId}" ${checked}>
                        <label class="${checkBoxId} custom-control-label" for="${checkBoxId}">${itemText}</label>
                 </li>`).appendTo($(this.dropDownMenu));
            if (isChecked) {
                this.appendToSelectedItems(checkBoxId, itemText);
            }
        }

        appendToSelectedItems(checkBoxId, itemText) {
            var $item = $(`<li class="badge pl-0 pt-1" data-option-id="${checkBoxId}">${itemText}</li>`).insertBefore($(this.filterInputItem)); // style="line-height: 1.5rem"
            var $buttom = $("<button class='close' aria-label='Close' type='button' style='font-size: 100%'><span aria-hidden='true'>&times;</span></button>")
                //.css(this.options.deleteBadgeButtonStyle)
                .appendTo($item); //btn btn-sm bg-transparent
            $buttom.click((event) => { this.removeItem($(event.currentTarget).parent().data('option-id')); });
        }

        init() {
            var $input = $(this.input);
            $input.hide();

            var $container = $("<div/>")
                .addClass(this.options.containerClass).insertAfter($input);
                
            this.container = $container.get(0);

            var $selectedPanel = $("<ul class='d-flex flex-row align-self h-input' data-toggle='dropdown' style='flex-wrap: wrap;' />")
                .addClass(this.options.selectedPanelClass)
                .appendTo($container);
            this.selectedPanel = $selectedPanel.get(0);

            var $filterInputItem = $('<li class="badge pl-0 pt-1"/>').appendTo($selectedPanel);
            this.filterInputItem = $filterInputItem.get(0)

            var $filterInput = $('<input class="border-0" autocomplete="off" style="outline: none; " type="text">').appendTo($filterInputItem);
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

            $(this.dropDownMenu).click( event => {
                event.stopPropagation();
            });

            $(document).mouseup((event) => {
                this.close(event);
            });


            $(this.dropDownMenu).find('.dropdown-item').click((event)=> {
                this.filter(event);
            });

            $(this.selectedPanel).click((event) => {
                console.log('selectedPanel click')
                $(this.selectedPanel).find('input').val('').focus();
                //this.filter(event);
            });

            $filterInput.on('click', (event) => {
                this.showDropDown();
            });

            // Set on change for filter input
            $filterInput.on('keyup focus', (event) => {
                this.find(event);
            });
        }
    }

    var jQueryInterface = function (options) {
        return this.each(function () {
            let data = $(this).data(dataKey)
            const _config = typeof options === 'object' && options

            if (!data && /dispose|hide/.test(options)) {
                return
            }

            if (!data) {
                data = new Plugin(this, _config);
                $(this).data(dataKey, data)
            }

            if (typeof options === 'string') {
                if (typeof data[options] === 'undefined') {
                    throw new TypeError(`No method named "${options}"`)
                }
                data[options]()
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