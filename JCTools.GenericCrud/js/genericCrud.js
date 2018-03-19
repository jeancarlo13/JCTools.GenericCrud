/*jshint esversion: 6 */

var genericCrud =
  genericCrud ||
  (function() {
    "use strict";

    function addResponse(response) {
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
      $.ajax({
        url: this.dataset.url,
        method: "GET",
        success: addResponse
      });
    }

    function executeCommitAction() {
      var modal = $("#genericCrudModal"),
        form = modal[0].querySelector("form"),
        data = form ? $(form).serialize() : undefined;
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

          if (isJson && r.success === true) {
            window.location.replace(r.redirectUrl);
          } else if (isHtml) {
            addResponse(r);
          }
        }
      });
    }

    return {
      showModal: () => showModal.call(this),
      executeCommitAction: () => executeCommitAction.call(this) 
    };
  })();
