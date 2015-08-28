
$(document).ready(function () {
    BetList.GetBetListByRefNo(refno);
});
var BetList = {
    gridOpts: {
        tblContentId: 'tblContent',
        manualCopyHeader: true,
        isShowPager: false,
        fullWidth: true,
        fullHeight: true,
        gridWidth: 980, //=totalColumnWidth + totalColumn*4 +totalColumn
        gridHeight: 100,
        freezeWidth: 460, //=totalColumnWidth + (totalColumn-1)*4 + (totalColumn-1)
        numberFreezeCol: 3
    },
    GetBetListByRefNo: function (refno) {
       // var refno = BetList.trim($('#txtRefNo').val());
        try { parent.BlockUI(); } catch (e) { }
        $.ajax({
            type: "POST",
            url: window.rootUrl + "BetList/GetBetListByRefNo",
            data: 'refno=' + refno,
            success: function (result) {
                $("#betListContent").html(result);
                $("#BetlistByRefNo").initGrid(BetList.gridOpts);
                $('#BetlistByRefNo').removeClass("display-none").gridScroller(BetList.gridOpts);
                if ($('div.combinationLink')) {
                    var divReplace = '<span style="color: #0066CC;">' + $('div.combinationLink a:first').html() + '</span>';
                    $('div.combinationLink a:first').replaceWith(divReplace);
                }
                if ($('#choice_left a:first').html() == 'Mix Parlay') {
                    var divReplace = '<span style="color: #0066CC;">' + $('#choice_left a:first').html() + '</span>';
                    $('#choice_left a:first').replaceWith(divReplace);
                }

                var heightChoice = $("#choice_left").height();
                var heightStatus = $("#stautus").height();
                var heightComm = $("#comm").height();
                var heightOdd = $("#odd").height();
                var heightTemp = heightStatus > heightComm ? heightStatus : heightComm
                var heightRight = heightTemp > heightOdd ? heightTemp : heightOdd;
                if (heightRight > heightChoice) {
                    $("#choice_left").attr('style', 'height:' + heightRight + 'px');
                }


                $("#hidden0").removeAttr("href");
                $("#result").attr('style', 'cursor: pointer;cursor: hand;');
            },
            error: function () {
                try { parent.jQuery.unblockUI(); } catch (e) { }
            },
            complete: function () {
                try { parent.jQuery.unblockUI(); } catch (e) { }
            }
        });
    },
    trim: function (str) {
        return str.replace(/(^\s*)|(\s*$)/g, "");
    }
};



function showCombMPDetail(tranid, date, refno, betid, matchid) {
    var username = $(".combinationLink span:last").html();
    showDetailsCMP(tranid, "System Parlay - " + username, parseFloat(refno), parseInt(betid), _custid)
}


