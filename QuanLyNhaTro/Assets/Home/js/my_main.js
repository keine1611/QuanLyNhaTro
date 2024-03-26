
    $('.checkbox_item').on('change', function () {
        console.log('lêuleeu');
    })


$('.btn_thanhtoan').on('click', function () {
    alert('adfasdf')
});


function checkClass() {
    if ($('#shoppingcart-modal'))
        console.log('co')
    else
        console.log('k')
};
checkClass();
$('#shoppingcart-modal').ready(function () {
    checkClass();
})