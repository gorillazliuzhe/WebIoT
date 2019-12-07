var gap = 3.75
var n = 49;
var currentData = 23;
var max = 50;
var min = 0;
var startDeg = -90
var deg;


$(document).ready(function () {
    for (var i = 0; i < n; i++) {
        deg = gap * i + startDeg;
        $("#kedu").append(" <div class=\"absolute ruler\" style=\"z-index:" + i + "; transform: rotate(" + deg + "deg);\">\n" +
            "            <div class=\"absolute ruler-deg ruler-bg\" style=\"overflow: hidden; z-index: " + i + ";\"></div>\n" +
            "            </div>")
    }
    var keduDeg = 10 * gap
    var textdeg;
    var text = min;
    var y = (max - min) / 5
    for (var j = 0; j < 6; j++) {
        textdeg = keduDeg * j - 90
        var next = 360 - textdeg

        $("#kedu-text").append("<div class=\"absolute kedu-text\"  style=\"z-index:" + j + " ; transform: rotate(" + textdeg + "deg);\">\n" +
            "        <div class=\"kedu-text-item\" style=\"transform: rotate(" + next + "deg); border-radius: 0px; white-space: pre; z-index:" + j + ";\">" + (text + y * j) + "</div></div>")
    }
    loadData(currentData);

})

function loadData(data) {
    $(".ruler-text").html(data + "<span>℃</span>")
    var left = $("#kedu").width() / 2 - $(".ruler-text").width() / 2;
    $(".ruler-text").css("left", left)
    $(".ruler-title").css("left", $("#kedu").width() / 2 - $(".ruler-title").width() / 2);
    $(".ruler-title").css("bottom", -8 - $(".ruler-title").height());
    $(".kedu-text-item").each(function (index, item) {
        if (index / 6 <= (data / (max - min))) {
            $(".kedu-text-item").eq(index).css("opacity", "1")
        } else {
            $(".kedu-text-item").eq(index).css("opacity", "0.5")
        }
    });

    $(".ruler-deg").each(function (index, item) {
        if (index * 3.75 / 180 <= (data / (max - min))) {
            $(".ruler-deg").eq(index).addClass("ruler-active")
            $(".ruler-deg").eq(index).removeClass("ruler-bg")
        } else {
            $(".ruler-deg").eq(index).addClass("ruler-bg")
            $(".ruler-deg").eq(index).removeClass("ruler-active")
        }
    })

}