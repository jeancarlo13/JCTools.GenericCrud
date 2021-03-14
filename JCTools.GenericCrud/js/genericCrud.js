/*jshint esversion: 6 */

var genericCrud = genericCrud || (function () {
  "use strict";

  var global = {
    loadingClass: 'fa fa-spinner fa-spin',
    fontAwesomeIsLoaded: true
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

  /**
   * Find the children elements that are comment blocks
   * @param {element} el The parent element where make the search
   * @param {bool} recursive True for a recursive search; else, false 
   */
  function findComments(el, recursive = false) {
    var arr = [];
    for (var i = 0; i < el.childNodes.length; i++) {
      var node = el.childNodes[i];
      if (node.nodeType === Node.COMMENT_NODE) {
        arr.push(node);
      } else if (recursive) {
        arr.push.apply(arr, findComments(node));
      }
    }
    return arr;
  };

  /**
   * Allows find the i element replaced by a svg by font awesome 5.x
   * @param {element} el The parent element where make the search
   * @returns {element} the found i HTML element
   */
  function findFaInComments(el) {
    let comments = findComments(el);
    let i = comments[0].nodeValue;
    i = i.substring(0, i.indexOf('</i>') + 4);
    el.innerHTML = i;
    i = el.querySelector('i');
    return i;
  }

  function showModal() {
    let self = this;
    let classes;
    if (global.fontAwesomeIsLoaded == true) {
      let i = self.querySelector('i') || findFaInComments(self);

      classes = i.className;
      i.className = global.loadingClass;
    }

    $.ajax({
      url: this.dataset.url,
      method: "GET",
      success: function (response) {
        addResponse(response);
        if (global.fontAwesomeIsLoaded == true) {
          i = self.querySelector('i') || findFaInComments(self);
          i.className = classes;
        }
      }
    });
  }

  function executeCommitAction() {
    let self = this,
      modal = $("#genericCrudModal"),
      form = modal[0].querySelector("form"),
      data = form ? $(form).serialize() : undefined,
      i = document.createElement('i');

    i.className = global.loadingClass;
    self.insertBefore(i, self.childNodes[0]);

    $.ajax({
      method: form && data ? "POST" : "GET",
      data: data,
      url: self.dataset.url,
      success: (r, status, xhr) => {
        var contentType = xhr.getResponseHeader("content-type"),
          isJson = contentType.indexOf("json") > -1,
          isHtml = contentType.indexOf("html") > -1;

        try {
          self.removeChild(i);
        } catch {
          let toRemove = Array.prototype.slice.call(self.querySelectorAll('svg'));
          if (toRemove.length > 1) {
            toRemove.pop();
          }
          toRemove.forEach(r => self.removeChild(r));
        }

        document.body.removeChild(modal[0]);
        modal.modal("hide");

        if (isJson && r.success === true) {
          window.location.replace(r.redirectUrl);
        } else if (isHtml) {
          addResponse(r);
        }
      }
    });
  }

  /**
   * Check if the font awesome is loaded in the page; if it's not loaded the button icons is replaced. 
   */
  function isLoadedFontAwesome() {
    /**
     * Gets the value of a css property from the browser computed style.
     * @param {Element} element The element with the style to check.
     * @param {string} property the name of the desired css property.
     * @returns {object} The found value of the property css applied to the specified element.
     */
    function css(element, property) {
      return window.getComputedStyle(element, null).getPropertyValue(property);
    }

    let span = document.createElement('span');

    span.classList.add('fa');
    span.style.display = 'none';
    document.body.insertBefore(span, document.body.firstChild);

    if ((css(span, 'font-family')).indexOf('Awesome') === -1) {
      document.body.removeChild(span);
      console.warn('The font awesome is not found. The button with font icons are replaced with text labels.');
      console.warn('Add the font awesome css from yourself location or use <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.2/css/all.min.css" integrity="sha512-HK5fgLBL+xu6dm/Ii3z4xhlSUyZgTT9tuc/hSrtw6uzJOvgRr2a9jyxxT1ely+B+xFAmJKVSTbpM/CuL7qxO8w==" crossorigin="anonymous" />');
      console.warn('Add the font awesome js from yourself location or use <script src="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.2/js/all.min.js" integrity="sha512-UwcC/iaz5ziHX7V6LjSKaXgCuRRqbTp1QHpbOJ4l1nw2/boCfZ2KlFIqBUA/uRVF0onbREnY9do8rM/uT/ilqw==" crossorigin="anonymous"></script>');
      document.querySelectorAll('.fa').forEach(i => {
        let text = i.dataset.text;
        i.parentElement.innerHTML = text;
      });

      global.fontAwesomeIsLoaded = false;
    } else {
      document.body.removeChild(span);
      console.log('Font awesome was found.');
      global.fontAwesomeIsLoaded = true;
    }
  };

  window.onload = isLoadedFontAwesome;

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