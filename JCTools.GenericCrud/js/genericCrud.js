/*jshint esversion: 6 */

var genericCrud =
  genericCrud ||
  (function () {
    "use strict";

    var global = {
      loadingClass: 'fa fa-spinner fa-spin'
    };

    function addResponse(response) {
      $("#genericCrudModal").modal("hide");
      $('.modal-backdrop.fade.show').remove();

      var div = document.createElement("div");
      div.innerHTML = response;

      let modal = div.querySelector(":first-child");
      document.body.appendChild(modal);

      let form = modal.querySelector("form");
      if (form) {
        form.onsubmit = evt => {
          evt.preventDefault();
          modal.querySelector(".modal-footer button:last-child").click();
          return false;
        };
      }

      $(modal).modal();
    }

    function showModal() {
      var i = this.querySelector('i'),
        classes = i.className;

      i.className = global.loadingClass;

      $.ajax({
        url: this.dataset.url,
        method: "GET",
        success: function (response) {
          addResponse(response);
          i.className = classes;
        }
      });
    }

    function executeCommitAction() {
      var modal = $("#genericCrudModal"),
        form = modal[0].querySelector("form"),
        data = form ? $(form).serialize() : undefined,
        i = document.createElement('i');

      i.className = global.loadingClass;
      this.insertBefore(i, this.childNodes[0]);

      $.ajax({
        method: form && data ? "POST" : "GET",
        data: data,
        url: this.dataset.url,
        success: (r, status, xhr) => {
          var contentType = xhr.getResponseHeader("content-type"),
            isJson = contentType.indexOf("json") > -1,
            isHtml = contentType.indexOf("html") > -1;

          modal.modal("hide");
          document.body.removeChild(modal[0]);

          i.parentElement.removeChild(i);
          if (isJson && r.success === true) {
            window.location.replace(r.redirectUrl);
          } else if (isHtml) {
            addResponse(r);
            let parent = i.parentElement;
            if (parent) {
              parent.removeChild(i);
            }
          }
        }
      });
    }

    return {
      showModal: function () {
        $("#genericCrudModal").remove();
        showModal.call(this);
      },
      executeCommitAction: function () {
        executeCommitAction.call(this);
      }
    };
  })();