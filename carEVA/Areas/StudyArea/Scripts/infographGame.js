function draggableOnDragStart(e) {
    //fired when we click the draggable and start moving
    //fired by the draggable
}

function draggableOnDragEnd(e) {
    //fired by the draggable when we drop the element
    var draggable = $("#draggable");

    if (!draggable.data("kendoDraggable").dropped) {
        // drag ended outside of any droptarget
    }
}

function droptargetOnDragEnter(e) {
    //fired when we enter one of the active zones e.dropTarget
    if ($(e.dropTarget).attr("data-eva-result") == "starting") {
        $(e.dropTarget).children("div").toggleClass("eva-feedback-infograph");
    }
    //e.dropTarget.toggleClass("eva-feedback-infograph");
}

function droptargetOnDragLeave(e) {
    //fired when we leave one of the active zones, the active zone is e.dropTarget
    if ($(e.dropTarget).attr("data-eva-result") == "starting") {
        $(e.dropTarget).children("div").toggleClass("eva-feedback-infograph");
    }
    //e.dropTarget.toggleClass("eva-feedback-infograph");
}

function droptargetOnDrop(e) {
    if ($(e.dropTarget).attr("data-eva-result") != "starting") {
        //do nothing if the target has already being played
        return;
    }
    //fired when we drop the draggable element
    var temp1 = $(e.draggable.element).attr("data-eva-match");
    var temp2 = $(e.dropTarget).attr("data-eva-match");

    if ($(e.draggable.element).attr("data-eva-match") == $(e.dropTarget).attr("data-eva-match")) {
        //matched the correct element, so show the correct message
        $(e.dropTarget).children("div").children("p.eva-correct-message-title").toggleClass("eva-hide-message");
        $(e.dropTarget).children("div").children("p.eva-correct-message").toggleClass("eva-hide-message");
        $(e.dropTarget).attr("data-eva-result", "correct");
        //$(e.dropTarget).children("div").html("<p>MUY BIEN ACERTASTE</p>");
    } else {
        $(e.dropTarget).children("div").children("p.eva-incorrect-message-title").toggleClass("eva-hide-message");
        $(e.dropTarget).children("div").children("p.eva-incorrect-message").toggleClass("eva-hide-message");
        $(e.dropTarget).attr("data-eva-result", "incorrect");
        //$(e.dropTarget).children("div").html("<p>AQUI NO CORRESPONDE</p>");
    }
}



$(document).ready(function () {
    $("#draggable").kendoDraggable({
        hint: function () {
            return $("#draggable").clone();
        },
        dragstart: draggableOnDragStart,
        dragend: draggableOnDragEnd
    });

    $("#draggable2").kendoDraggable({
        hint: function () {
            return $("#draggable2").clone();
        },
        dragstart: draggableOnDragStart,
        dragend: draggableOnDragEnd
    });

    $("#droptarget").kendoDropTargetArea({
        filter: ".eva-infograph",
        dragenter: droptargetOnDragEnter,
        dragleave: droptargetOnDragLeave,
        drop: droptargetOnDrop
    });
});