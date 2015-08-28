/* File Created: December 13, 2012 */
$(document).ready(function () {
    var myGrid = new FreezeGrid();
    myGrid.initPage();
});
function FreezeGrid() {
    var seft = this;
    seft.initPage = function () {
        initGridScrollerTables();
        var t = setTimeout(function () {
            $("#mygrid").gridScroller(options);
        }, 10);
    };
    initGridScrollerTables = function () {
        $("#mygrid").initGrid({
            isShowPager: true
        });

    };
    var options = {
        isShowPager: true,
        tblContentId: 'grid_content',
        fullHeight: true,
        fullWidth: true,
        gridWidth: 1300,
        gridHeight: 150,
        freezeWidth: 290,
        freezeHeight: 300,
        numberFreezeCol: 3
    };
};