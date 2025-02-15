/*
	Template Name: circular - 
	
	Author URI: https://Bizbrolly.com
	Version: 1.0
  
*/


jQuery(function($) {

    // ======================DataTable JS===============================
    
    /*=======================Community Management=======================*/
      otable9 = $('#active-communities-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })

           otable9 = $('#marketing-statistics-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
           otable9 = $('#half-communties-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
   // =====================community-profile========================
   otable9 = $('#members-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })

          
           otable9 = $('#access-control-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
            otable9 = $('#groups-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
             otable9 = $('#transactional-history-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
              otable9 = $('#edit-custom-groups-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
          // =======================member management===========================
           otable9 = $('#member-database-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
           otable9 = $('#affiliate-database-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
              
             /*=======================Poll Result=================================*/
       otable9 = $('#poll-result-table').DataTable({
              "order": [[0, "desc"]],
              searching: false,
              pagination: false,

              "dom": 'rt<"bottom"flp><"bottom"i>'
          })
             // ======================DataTable END===============================

// ===========================genral tab in community-profile=========================
             $("#custom-landing").click(function(){
                 $(".create-custom-landing").show();
                 $(".general-profile").hide();
                 $(".edit-general-profile").hide();
                 $(".update-custom-landing").hide();
                });
             $("#create-landing-btn").click(function(){
                  $(".edit-general-profile").show();
                  $(".create-custom-landing").hide();
                  $(".general-profile").hide();
                  $(".update-custom-landing").hide();
                });
             $("#edit-custom-landing-btn").click(function(){
                  $(".update-custom-landing").show();
                  $(".create-custom-landing").hide();
                  $(".general-profile").hide();
                  $(".edit-general-profile").hide();
                });
             $("#update-landing-btn").click(function(){
                  $(".edit-general-profile").show();
                   $(".general-profile").hide();
                  $(".create-custom-landing").hide();
                  $(".update-custom-landing").hide();
                });

             $("#cencel-custom-landing-btn").click(function(){
                  $(".general-profile").show();
                   $(".edit-general-profile").hide();
                  $(".create-custom-landing").hide();
                  $(".update-custom-landing").hide();
                });
             $("#cencel-landing-btn").click(function(){
                  $(".edit-general-profile").show();
                   $(".general-profile").hide();
                  $(".create-custom-landing").hide();
                  $(".update-custom-landing").hide();
                });

             // =================Group Tab in Community Profile============
   
             
// =====================datepicker==========================
       $("#datefrom").datepicker({

        dateFormat: 'dd-mm-yy',

        beforeShow: function (input, inst) {
    $('.overlay').addClass('d-block');
},
onSelect: function (dateText, inst) {
    $('.overlay').removeClass('d-block');
}
    });

// $("#transaction-date").datepicker({

//     dateFormat: 'dd-mm-yy',
//     Range: true
//        beforeShow: function (input, inst) {
//    $('.overlay').addClass('d-block');
//},
//onSelect: function (dateText, inst) {
//    $('.overlay').removeClass('d-block');
//}
//    });
//       $(".overlay").on("click", function () {
//    $(".overlay").removeClass("d-block");
//});


$("#schedule_date").datepicker({

        dateFormat: 'dd-mm-yy',

        beforeShow: function (input, inst) {
    $('.overlay').addClass('d-block');
},
onSelect: function (dateText, inst) {
    $('.overlay').removeClass('d-block');
}
    });



$("#edit_schedule_date").datepicker({

        dateFormat: 'dd-mm-yy',

        beforeShow: function (input, inst) {
    $('.overlay').addClass('d-block');
},
onSelect: function (dateText, inst) {
    $('.overlay').removeClass('d-block');
}
    });

// ================end================

// ===============overlay=================
       $(".overlay").on("click", function () {
    $(".overlay").removeClass("d-block");
});
// ==================end====================


    $('#poll-back').click(function (e) {
        $('.tab-data-area a[href="#PollResult"]').tab("show");

    })
    
     $('.tab-data-area a[href="#MarketingStatistics"]').click(function (e) {
         $(".main-top-area").addClass("intro");

    })

     $('.tab-data-area a').click(function() {
        if ($(this).hasClass('height-main-top')){
          $(".main-top-area").addClass("intro");
        } else {
            $(".main-top-area").removeClass("intro");
          }
        });

    
   

});



// ==============================datepicker===========================

    