/*
 * jQuery wmd plugin.
 */

(function($) {
    var counter = 0;
    
    $.fn.wmd = function(_options) {
        this.each(function() {
            var defaults = {"preview": true};
            var options = $.extend({}, _options || {}, defaults);
            
            if (!options.button_bar) {
                options.button_bar = "wmd-button-bar-" + counter;
                $("<div/>")
                    .attr("class", "wmd-button-bar")
                    .attr("id", options.button_bar)
                    .insertBefore(this);
            }
                
            if (typeof(options.preview) == "boolean" && options.preview) {
                options.preview = "wmd-preview-" + counter;
                $("<div/>")
                    .attr("class", "wmd-preview")
                    .attr("id", options.preview)
                    .insertAfter(this);
            }

            if (typeof(options.output) == "boolean" && options.output) {
                options.output = "wmd-output-" + counter;
                $("<div/>")
                    .attr("class", "wmd-output")
                    .attr("id", options.output)
                    .insertAfter(this);
            }
                
            this.id = this.id || "wmd-input-" + counter;
            options.input = this.id;
            
            setup_wmd(options);
            counter++;
        });
    };
})(jQuery);
