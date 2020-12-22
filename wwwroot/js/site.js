// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});


addtoCart2 = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                $.notify({
                    message: res,
                    type: 'success'
                });
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}


/*Mua hàng thành công*/
success = (url) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            window.location.href = url;
        }
    })
}

/*Show menu*/
showMenu = (url) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#menu_partial .menu-drop').html(res);
        }
    })
}


/**Show Popup Modal*/
showInPopup = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal .modal-title').html(title);
            $('#form-modal').modal('show');
        }
    })
}


addItem = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $.notify({
                message: res,
            }, {
                offset: {
                    y: 80,
                    x: 20,
                },
                delay: 500,
                type: 'success',
                url_target: '_self',
            });
        }
    })
}

CancelOrder = (url) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $.notify({
                message: res,
            }, {
                offset: {
                    y: 80,
                    x: 20,
                },
                delay: 500,
                type: 'success',
                url_target: '_self',
            });
            $('#form-modal').modal('hide');
        }
    })
}



DeleteItem = (url, title, id) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#product' + id).parents('tr').remove();
            $.notify({
                message: res,
            }, {
                offset: {
                    y: 80,
                    x: 20,
                },
                delay: 500,
                type: 'success',
                url_target: '_self',
            });
        }
    })
}

EditItem = (url, title) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $.notify({
                message: res,
            }, {
                offset: {
                    y: 80,
                    x: 20,
                },
                delay: 500,
                type: 'success',
                url_target: '_self',
            });
            loadCart('/CartDetail/loadCart');
        }
    })
}


/*Show data Card - Home Page*/
showDataView = (url) => {
    $.ajax({
        async: true,
        type: 'GET',
        url: url,
        success: function (res) {
            $('#card-view .card-body').html(res);
        }
    })
}

showDataSale = (url) => {
    $.ajax({
        async: true,
        type: 'GET',
        url: url,
        success: function (res) {
            $('#card-sale .card-body').html(res);
        }
    })
}

showDataFavo = (url) => {
    $.ajax({
        async: true,
        type: 'GET',
        url: url,
        success: function (res) {
            $('#card-favo .card-body').html(res);
        }
    })
}

/*Show Cart Dropdown*/
showCart = (url) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#dropdown-cart .dropdown-body').html(res);
        }
    })
}

showFavorite = (url) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#dropdown-favorite .dropdown-body').html(res);
        }
    })
}

loadCart = (url) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#table-cart').html(res);
        }
    })
}

/*Show order*/
showOrder = (url, i) => {
    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
            $('#cartorder' + i +' .card-body').html(res);
        }
    })
}