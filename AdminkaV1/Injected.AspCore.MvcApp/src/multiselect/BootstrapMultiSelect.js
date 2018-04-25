

// Double IIFE style JQuery plugin (Immediately-Invoked Function Expression)
// First IIFE for search module managers, second to protect $
; (function (window, document) {
    var pluginName = "bootstrapMultiSelect";
    (function (factory) {
        "use strict";

        if (typeof define === 'function' && define.amd) {
            // Define as an AMD module if possible
            define(pluginName, ['jquery'], factory);
        }
        else if (typeof exports === 'object') {
            // Node/CommonJS
            module.exports = factory(require('jquery'));
        }
        // use global (browser's) otherwise
        else if (jQuery) {
            // But prevent multiple initializations (installations in term of jquery plugin)
            if (!jQuery.fn.bootstrapMultiSelect) {
                factory(jQuery);
            }
        }
    }(
        // factory definition
        function ($) {
            "use strict";
            var defaults = {
                multi_select: true,
                items: [],
                defaults: [],
                filter_text: 'Filter',
                rtl: false,
                case_sensitive: false
            };


            // Plugin constructor
            function Plugin(element, options) {
                this.element = element;
                this.options = $.extend({}, defaults, options);
                this._defaults = defaults;
                this._name = pluginName;

                this.$input = $(element);
                this.$container = null;
                this.multiselectId = 100500;
                this.dropDown = null;
                this.$selectedItems = null;

                // public methods
                this.removeItem = function (id) {
                    var $multi_select = $('#bootstrap-multiselect-container-' + id.split('-')[0]);
                    $multi_select.find('[data-val="' + id + '"]').remove();
                    $multi_select.find('[id="' + id + '"]').prop('checked', false);
                    if ($multi_select.find('.selected-items > .item').length < 1) {
                        $multi_select.find('.selected-items > .placeholder').show();
                    }
                }

                this.init();
            }

            Plugin.prototype = {
                init: function () {
                    var multiselectId = this.multiselectId;
                    var $input = this.$input;
                    var options = this.options;
                    var removeItem = this.removeItem;

                    $input.css({ 'display': 'none', 'position': 'absolute' });
                    
                    this.select = $('<div class="bootstrap-multiselect-container" id="bootstrap-multiselect-container-' + multiselectId + '">' +
                        '<div class="selected-items form-control">' +
                        '<span class="placeholder">' + $input.attr('placeholder') + '</span>' +
                        '</div>' +
                        '<div class="dropdown form-control">' +
                        '<div class="filter">' +
                        '<input type="text" class="form-control" placeholder="' + options.filter_text + '">' +
                        '<button type="button">&times;</button>' +
                        '</div>' +
                        '<div class="items"></div>' +
                        '</div>' +
                        '</div>').insertAfter($input);
                    this.select.find('.filter button').click(function(){
                        $(this).parent().find('input').val('').focus();
                    });
                    
                    this.$container = $(".bootstrap-multiselect-container");
                    var $container = this.$container;
                    var dropDown = $container.find('.dropdown');
                    this.dropDown = dropDown;
                    var $selectedItems = $container.find('.selected-items');
                    this.$selectedItems = $selectedItems;

                    if (options.defaults.length > 0) {
                        $selectedItems.find('.placeholder').hide();
                    }

                   
                    this.options.items.forEach(function (item) {
                        var $items = $container.find('.items');

                        if (options.defaults.includes(item['value'])) {
                            $items.append(
                                '<div class="item">' +
                                '<div class="custom-control custom-checkbox">' +
                                '<input type="checkbox" class="custom-control-input" id="' + multiselectId + '-chbx-' + item['value'] + '" checked>' +
                                '<label class="custom-control-label ' + multiselectId + '-chbx-' + item['value'] + '" for="' + multiselectId + '-chbx-' + item['value'] + '">' + item['text'] + '</label>' +
                                '</div>' +
                                '</div>'
                            );

                            $selectedItems.append(
                                '<span class="item" data-val="' + multiselectId + '-chbx-' + item['value'] + '">' + item['text'] +
                                '<button type="button">&times;</button>' +
                                '</span>'
                            );
                            $selectedItems.find('button:last-child').click(function () {
                                console.log(this);
                                removeItem($(this).parent().data('val'));
                            });
                        } else {
                            $items.append(
                                '<div class="item">' +
                                '<div class="custom-control custom-checkbox">' +
                                '<input type="checkbox" class="custom-control-input" id="' + multiselectId + '-chbx-' + item['value'] + '">' +
                                '<label class="custom-control-label ' + multiselectId + '-chbx-' + item['value'] + '" for="' + multiselectId + '-chbx-' + item['value'] + '">' + item['text'] + '</label>' +
                                '</div>' +
                                '</div>'
                            );
                        }
                    });

                    dropDown.click(function (e) {
                        e.stopPropagation();
                    });

                    // close popup there
                    $(document).mouseup(function (e) {
                        if (!$container.is(e.target) && $container.has(e.target).length === 0) {
                            $selectedItems.removeClass('expand');
                            dropDown.removeClass('expand');
                        }
                    });

                    
                    dropDown.find('.item').click(function (e) {
                        
                        e.stopPropagation();
                        e.preventDefault();

                        var item = $(this);
                        var inputElem = item.find('input');

                        // Hide placeholder
                        if ($selectedItems.find('.item').length < 1) {
                            $selectedItems.find('.placeholder').hide();
                        }


                        // if the item is already checked
                        if (inputElem.prop('checked')) {
                            $selectedItems.find('[data-val="' + inputElem.attr('id') + '"]').remove();

                            // uncheck item
                            inputElem.prop('checked', false);

                            // Set placeholder
                            if ($selectedItems.find('.item').length < 1) {
                                $selectedItems.find('.placeholder').show();
                            }
                        } else {
                            // if multi-select option is off
                            if (!options.multi_select) {
                                dropDown.find('.item input').prop('checked', false);
                                $container.find('.selected-items > .item').remove();
                            }
                            // Check item
                            inputElem.prop('checked', true);

                            var $selectedItems2 = $container.find('.selected-items');
                            $selectedItems2.append(
                                '<span class="item" data-val="' + inputElem.attr('id') + '">' + item.find('label').html() +
                                '<button type="button">&times;</button>' +
                                '</span>'
                            );
                            $selectedItems2.find('button:last-child').click(function () {
                                console.log(this);
                                removeItem($(this).parent().data('val'));
                            })
                        }
                    });

                    // Set onclick for select to expand dropDown
                    $container.on('click', function () {
                        dropDown.addClass('expand');
                        $selectedItems.addClass('expand');
                    });

                    // Set on change for filter input
                    $container.find('input[type="text"]').on('keyup focus', function () {
                        var filter = $(this);
                        var text = filter.val();

                        if (options.case_sensitive) {
                            dropDown.find('.item').each(function () {
                                var item = $(this);
                                if (item.html().includes(text)) {
                                    item.show();
                                } else {
                                    item.hide();
                                }
                            });
                        } else {
                            dropDown.find('.item').each(function () {
                                var item = $(this);
                                if (item.html().toLowerCase().includes(text.toLowerCase())) {
                                    item.show();
                                } else {
                                    item.hide();
                                }
                            });
                        }
                    });

                    return $container;
                },

                get_items: function () {
                    var items = [];
                    var $container = this.$container;
                    $container.find('.dropdown').find('input').each(function () {
                        var item = $(this);

                        if (item.prop('checked')) {
                            items.push(item.attr('id').split('-')[2]);
                        }
                    });

                    return items;
                }
                
            };

            //$.fn[pluginName] = function (options) {

            //    if (methods[options]) {
            //        return methods[options].apply(this, Array.prototype.slice.call(arguments, 1));
            //    } else if (typeof options === 'object' || !options) {
            //        // Default to "init"
            //        return methods.init.apply(this, arguments);
            //    } else {
            //        $.error('Method ' + options + ' does not exist on jQuery.bootstrapMultiSelect');
            //    }
            //};

            // A really lightweight plugin wrapper around the constructor,
            // preventing multiple instantiations
            $.fn[pluginName] = function (options) {
                return this.each(function () {
                    if (!$.data(this, "plugin_" + pluginName)) {
                        $.data(
                            this,
                            "plugin_" + pluginName,
                            new Plugin(this, options)
                        );
                    }
                });
            };

            $.fn[pluginName.charAt(0).toUpperCase() + pluginName.slice(1)] = function (options) {
                return $(this).data("plugin_" + pluginName);
            };

            return $.fn.bootstrapMultiSelect;
        }
    )
    );
}(window, document));
