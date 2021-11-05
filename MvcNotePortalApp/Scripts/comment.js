var noteId = -1;

function doComment(btn, mode, commentId, commentTextId) {
    var button = $(btn); // elementi jquery elementine çevirdik
    var editMode = button.data("edit-mode");

    if (mode === "edit") {

        if (!editMode) {
            button.data("edit-mode", true); // data-edit-mode attribute'unu true yaptık
            button.removeClass("btn-warning");
            button.addClass("btn-success");

            var btnSpan = button.find("span");
            btnSpan.removeClass("bi-pencil-fill");
            btnSpan.addClass("bi-check");

            $(commentTextId).parent().addClass("editable");
            $(commentTextId).addClass("no-outline");
            $(commentTextId).attr("contenteditable", true);
            $(commentTextId).focus();
        }
        else {
            button.data("edit-mode", false);
            button.removeClass("btn-success");
            button.addClass("btn-warning");

            var btnSpan = button.find("span");
            btnSpan.removeClass("bi-check");
            btnSpan.addClass("bi-pencil-fill");

            $(commentTextId).removeClass("editable");
            $(commentTextId).attr("contenteditable", false);

            var txt = $(commentTextId).text();

            $.ajax({
                method: "POST",
                url: "/Comment/Edit/" + commentId,
                data: { text: txt }
            }).done(function (data) {
                if (data.result) {
                    $("#modalComment_modalCommentBody").load("/Comment/ShowNoteComments/" + noteId);
                }
                else {
                    alert("Yorum güncellenemedi.");
                }
            }).fail(function () {
                alert("Sunucu ile bağlantı kurulamadı.");
            });
        }
    }
    else if (mode === "delete") {
        var dialog_res = confirm("Yorum silinsin mi?");
        if (!dialog_res) return false;

        $.ajax({
            method: "GET",
            url: "/Comment/Delete/" + commentId,
        }).done(function (data) {
            if (data.result) {
                $("#modalComment_modalCommentBody").load("/Comment/ShowNoteComments/" + noteId);
            }
            else {
                alert("Yorum silinemedi.");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });
    }
    else if (mode === "new") {

        var txt = $("#new_comment_text").val();

        $.ajax({
            method: "POST",
            url: "/Comment/Create",
            data: { text: txt, "noteId": noteId }
        }).done(function (data) {
            if (data.result) {
                $("#modalComment_modalCommentBody").load("/Comment/ShowNoteComments/" + noteId);
                $("#new_comment_text").val("");
            }
            else {
                alert("Yorum eklenemedi.");
            }
        }).fail(function () {
            alert("Sunucu ile bağlantı kurulamadı.");
        });
    }
}

$(function () {

    $('#modalComment').on('shown.bs.modal', function (e) {
        // bootstrap'da modal'ı tıklayarak getiren öğe "relatedTarget" olarak işaretlenir;
        var btn = $(e.relatedTarget); // buton'u jquery elementine çevirerek yakaladık

        noteId = btn.data("note-id");
        // data attribute'undakiler bu şekilde data metodu ile alınır ("data-.."'dan sonraki kısım yazılır)

        $("#modalComment_modalCommentBody").load("/Comment/ShowNoteComments/" + noteId);
    });

    $('#modalComment').on('show.bs.modal', function (e) {
        var btn = $(e.relatedTarget);

        var noteId = btn.data("note-id");

        $("#modalComment_modalNoteBody").load("/Note/ShowNoteModal/" + noteId);
    });

});