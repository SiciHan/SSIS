$(document).ready(function () {
    function onChange() {
        var checkbox = $("#QtyCheck");
        var checked = checkbox.is(':checked');
        var btnAck = $("#btnAck");
        if (checked) {
            btnAck.removeAttr("disabled");
        } else {
            btnAck.attr("disabled", true);
        }
    }

    function onViewMapClick(IdCollectionPt) {
        var value = IdCollectionPt;
        self.location.href = 'LocationMap?idCollectionPt=' + value;
    }
});
