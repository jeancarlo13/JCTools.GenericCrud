var genericCrud =
  genericCrud ||
  (function() {
    "use strict";

    function addResponse(response) {
      var div = document.createElement("div"),
        modal = undefined;

      div.innerHTML = response;
      modal = div.querySelector(":first-child");

      document.body.appendChild(modal);

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
      showModal: function() {
        showModal.call(this);
      },
      executeCommitAction: function() {
        executeCommitAction.call(this);
      }
    };
  })();
