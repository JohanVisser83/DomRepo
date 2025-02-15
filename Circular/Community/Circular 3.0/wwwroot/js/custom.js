/*
	Template Name: circular - 
	Author: Tripples
	Author URI: https://Bizbrolly.com
	Version: 1.0
  
*/


jQuery(function($) {
  "use strict";
$(document).ready(function(){
 $('.security-tab-click').click(function(e){
     $('.tab-data  a[href="#security-tab"]').tab('show');
    $('.tab-content #security-tab').tab('show');
})
});
$(document).ready(function(){
	   
		   $('.community-tab li a').hide();
		   $('.community-tab li:first-child a, .community-tab li:nth-child(2) a, .community-tab li:nth-child(3) a').show();

 $('#new-message').click(function(e){
     $('.tab-data  a[href="#new-mes"]').tab('show');
})
 $('#select_network').click(function(e){
    $('.community-tab  li a').hide();
	$('.community-tab li:nth-child(4) a').show();
	   $('.community-tab  a[href="#network"]').tab('show');
})
 $('#friends_tab').click(function(e){
    $('.community-tab  li a').hide();
	$('.community-tab li:first-child a').show();
	   $('.community-tab  a[href="#friends"]').tab('show');
	   
})
 $('#job_board_tab').click(function(e){
    $('.community-tab  li a').hide();
	$('.community-tab li:nth-child(5) a').show();
	$('.community-tab li:nth-child(6) a').show();
	   $('.community-tab  a[href="#job-posting"]').tab('show');
	   
})
$('#Community_tab').click(function(e){
	
    $('.community-tab  li a').hide();
	$('.community-tab li:nth-child(7) a, .community-tab li:nth-child(8) a').show();
	   $('.community-tab  a[href="#application-history"]').tab('show');
	   
})
});

$('#pp-card').owlCarousel({
   loop:true,
    margin:10,
    responsiveClass:true,
    dots:false,
    nav:false,
    responsive:{
        0:{
            items:1,
            nav:true
        },
        600:{
            items:2,
            nav:false
        },
        1000:{
            items:3,
            nav:true,
            loop:false
        }
    }
})

});