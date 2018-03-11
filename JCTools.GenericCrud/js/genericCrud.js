var genericCrud =
  genericCrud ||
  (function() {
    "use strict";

    function showModal() {
      $.ajax({
        url: this.dataset.url,
        method: "GET",
        success: function(response) {
          var div = document.createElement("div"),
            modal = undefined;

          div.innerHTML = response;
          modal = div.querySelector(":first-child");

          document.body.appendChild(modal);

          $(modal).modal();
        }
      });
    }

    return {
      showModal: function() {
        showModal.call(this);
      }
    };
  })();
