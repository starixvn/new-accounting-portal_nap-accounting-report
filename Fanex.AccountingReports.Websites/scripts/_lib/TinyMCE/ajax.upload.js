/*
 *
 * @name		ajaxMultiFileUpload
 * @author		QuanNTT
 * @type    	jQuery
 *
*/
; (function($) {

	$.fn.extend({
		ajaxUpload: function(options) { 
			opt = $.extend({}, $.fn.ajaxUpload.defaults, options);
			if (opt.file_types.match('jpg') && !opt.file_types.match('jpeg')) 
				opt.file_types += ',jpeg';
			$this = $(this);
			new $.fn.ajaxUpload.setup();
		}
	});

	$.fn.ajaxUpload.setup = function() {
        if($("#frmHide").length==0) {
		     $('body').append($('<div id="frmHide" style="display: none;"></div>').append($('<iframe width="0" height="0" src="about:blank" id="myFrame" name="myFrame"></iframe>')));
	    }

         if($("#myUploadForm").length==0) {
             $this.append($('<form target="myFrame" enctype="multipart/form-data" action="' + opt.ajaxFile + '" method="POST" name="myUploadForm" id="myUploadForm"><input type="hidden" name="fileName" id="fileName" value="uploadFileName" /></form>'));
             init();
       }
	};

	$.fn.ajaxUpload.defaults = {
		// image types allowed
		file_types: "jpg,jpeg,gif,png,bmp",
		// server script
		ajaxFile: "",
		// maximum number of files allowed to upload
		maxNumFiles: 1
	};

	function init() {
		// if file type is allowed, submit form
		$('#txtfile').bind('change', function() { 
			if (!checkFileType($(this).val())) {
			   $(this).val(''); 
			   whatNews.uploadMessage(whatNews.message.file_error, 5000);
			} 	
		});
		
		$("#btnUpload").bind("click", function(e) {
			if(whatNews.imageTab=='choose') { 	
				if (!checkFileType($('#txtfile').val()) || $('#txtfile').val()=="") {
				   $('#txtfile').val(''); 
				   whatNews.uploadMessage(whatNews.message.file_error, 5000);
				} else {
					var oldElement = $('#txtfile');
					var newElement = $(oldElement).clone();
					$(oldElement).attr('id', 'txtfile');
					$(oldElement).attr('name', 'txtfile');
					$(oldElement).attr('class', 'clone');
					$(oldElement).before(newElement);
					$(oldElement).appendTo('#myUploadForm');
					$('#myUploadForm').submit();
				} 
			} else if(whatNews.imageTab=='url') {
				var image_url = $.trim($("#txt_url").val());
				if(!checkFileType(image_url) || image_url=="") {
					whatNews.uploadMessage(whatNews.message.file_error, 5000);
					$('#txt_url').val(''); 
					$('#txt_url').focus(); 
				} else {
					var data = 'False|Success|'.concat(image_url);
					whatNews.uploadFile(data);
				}
			}
			return false;
		});

		// execute event.submit when form is submitted
		$('#myUploadForm').submit(function() { 
			return event.submit(this); 
		});
		// function to handle form submission using iframe
		var event = {
			// setup iframe
			frame: function(_form) {
				$("#myFrame")
					.empty()
					.one('load',  function() { event.loaded(this, _form) });
			},
			// call event.submit after submit
			submit: function(_form) {
				event.frame(_form);
			},
			// display results from submit after loades into iframe
			loaded: function(id, _form) {
				var d = frametype(id);
				try {
					var data = d.body.innerHTML.replace(/^\s+|\s+$/g, '');
					whatNews.uploadFile(data);
				} catch(ex) { }
			}
		};
		// check if file extension is allowed
		function checkFileType(file_) {
			var ext_ = file_.toLowerCase().substr(file_.toLowerCase().lastIndexOf('.') + 1);
			if (!opt.file_types.match(ext_)) {
				return false;
			} 
			else return true;
		};
		// check type of iframe
		function frametype(fid) {
			return (fid.contentDocument) ? fid.contentDocument: (fid.contentWindow) ? fid.contentWindow.document: window.frames[fid].document;
		};
	}
})(jQuery);