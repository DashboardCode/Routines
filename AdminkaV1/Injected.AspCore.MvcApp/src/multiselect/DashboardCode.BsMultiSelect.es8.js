import $ from 'jquery'
import Popper from 'popper.js'

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
        selectedPanelClass: "h-input form-control"

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
            this.popper = null;
            this.init();
        }

        // Public methods
        removeItem(optionId) {
            var $container = $(this.container);
            $container.find('[data-option-id="' + optionId + '"]').remove();
            $container.find('input[id="' + optionId + '"]').prop('checked', false);
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

        expand() {
            // TODO: if utilize BT dorpdown then us toggle
            //$(this.dropDownMenu).dropdown('toggle');
            $(this.dropDownMenu).addClass('show');
        }

        close(event) {
            var $container = $(this.container);
            if (!$container.is(event.target) && $container.has(event.target).length === 0) {
                // TODO: if utilize BT dorpdown then CHECK WHY 'toggle' method doesn't work (it is HUGE and non trivial in BS sources)
                // $(this.dropDownMenu).dropdown('toggle');
                $(this.dropDownMenu).removeClass('show');
            }
        }

        find(event) {
            var $filter = $(event.currentTarget);
            var text = $filter.val();

            if (this.options.case_sensitive) {
                $(this.dropDownMenu).find('.dropdown-item').each(function () {
                    var item = $(this);
                    if (item.html().includes(text)) {
                        item.show();
                    } else {
                        item.hide();
                    }
                });
            } else {
                $(this.dropDownMenu).find('.dropdown-item').each(function () {
                    var item = $(this);
                    if (item.html().toLowerCase().includes(text.toLowerCase())) {
                        item.show();
                    } else {
                        item.hide();
                    }
                });
            }
        }

        filter(event) {
            console.log("filter & stopPropagation");
            event.stopPropagation();
            event.preventDefault();

            var item = $(event.currentTarget);
            var $checkbox = item.find('input[type="checkbox"]');
            var checkBoxId = $checkbox.attr('id');
            
            if ($checkbox.prop('checked')) {
                $(this.selectedPanel).find(`span[data-option-id="${checkBoxId}"]`).remove();
                $checkbox.prop('checked', false);

            } else {
                var itemText = item.find('label').html();
                this.appendToSelectedItems(checkBoxId, itemText);
                $checkbox.prop('checked', true);
            }
        }

        appendToSelectedItems(checkBoxId, itemText) {
            var $item = $(`<span class="d-inline-block" data-option-id="${checkBoxId}">${itemText}</span>`).insertBefore($(this.filterInput));
            var $buttom = $("<button class='btn btn-sm bg-transparent' type='button'>&times;</button>").appendTo($item);
            $buttom.click((event) => { this.removeItem($(event.currentTarget).parent().data('option-id')); });
        }

        init() {
            var $input = $(this.input);
            $input.hide();

            var $container = $("<div/>")
                .addClass(this.options.containerClass).insertAfter($input);
                
            this.container = $container.get(0);

            var $selectedPanel = $("<div/>")
                .addClass(this.options.selectedPanelClass).appendTo($container);
            this.selectedPanel = $selectedPanel.get(0);

            var $filterInput = $('<input type="text">').appendTo($selectedPanel);
            this.filterInput = $filterInput.get(0)

            var $dropDownMenu = $(
                `<div class="dropdown-menu"/>`
            ).appendTo($container);
            this.dropDownMenu = $dropDownMenu.get(0);

            this.popper = new Popper(this.filterInput, this.dropDownMenu, {
                placement: 'bottom',
                modifiers: {
                    flip: {
                        behavior: ['left', 'right']
                    },
                },
            });
            // TODO: alternative is to use high level BS dropdown plugin (but it has less options)
            // this.dropdown = $dropDownMenu.dropdown({
            //     reference: this.filterInput,
            //     flip: true,
            // });
            
            this.options.items.forEach( item => {
                var itemValue = item['value'];
                var itemText = item['text'];
                var checkBoxId = 'item-value-' + itemValue;
                var isChecked = this.options.defaults.includes(item['value']);
                var checked = isChecked ? "checked" : "";
                var dropDownItem = $(
                    '<div class="dropdown-item"><div class="custom-control custom-checkbox">' +
                        `<input type="checkbox" class="custom-control-input" id="${checkBoxId}" ${checked}>` +
                        `<label class="custom-control-label ${checkBoxId}" for="${checkBoxId}">${itemText}</label>` +
                    '</div></div>').appendTo($(this.dropDownMenu));
                if (isChecked) {
                    this.appendToSelectedItems(checkBoxId, itemText);
                }
            });

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

            // Set onclick for select to expand dropDown
            $container.on('click', (event) => {
                this.expand();
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