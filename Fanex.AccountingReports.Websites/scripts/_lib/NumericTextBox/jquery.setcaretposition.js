$(document).ready(function(){
    if (typeof $ == "undefined") return;
    jQuery.fn.setCaretPosition = function(pos) {
        return this.each(function() {
            if(this.setSelectionRange) {
	            this.focus();
	            this.setSelectionRange(pos, pos);
            }
            else if (this.createTextRange) {
	            var range = this.createTextRange();
	            range.collapse(true);
	            range.moveEnd('character', pos);
	            range.moveStart('character', pos);
	            range.select();
            }
        });
    };
});