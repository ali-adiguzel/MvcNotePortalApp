$(function () {
    $('#modalNote').on('show.bs.modal', function (e) {
        var btn = $(e.relatedTarget);

        var noteId = btn.data("note-id");

        $("#modalNote_modalNoteBody").load("/Note/ShowNoteModal/" + noteId);
    });
});

