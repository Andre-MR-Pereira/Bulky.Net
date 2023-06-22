$('#Input_CompanyId').change(function () {
    var companyName = $(this).find(':selected').text()
    $.ajax({
        url: '?handler=UpdateWithCompanyDetails&companyName=' + companyName,
        success: function (data) {
            $('#Input_City').val(data.city);
            $('#Input_Name').val(data.name);
            $('#Input_PostalCode').val(data.postalCode);
            $('#Input_PhoneNumber').val(data.phoneNumber);
            $('#Input_State').val(data.state);
            $('#Input_StreetAddress').val(data.streetAddress);

            Swal.fire({
                icon: 'success',
                title: 'Found company!',
                text: 'Autofilling with information...'
            })
        },
        error: function (data) {
            Swal.fire({
                icon: 'error',
                title: 'No company...',
                text: 'Unable to fetch company information.'
            })
        }
    })
})