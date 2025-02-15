/*
	Template Name: circular - 
	
	Author URI: https://Bizbrolly.com
	Version: 1.0
  
*/


jQuery(function($) {

    // ======================DataTable JS===============================
    
   $('select').selectpicker();

    const header = document.querySelector(".header");
const toggleClass = "is-sticky";

window.addEventListener("scroll", () => {
  const currentScroll = window.pageYOffset;
  if (currentScroll > 150) {
    header.classList.add(toggleClass);
  } else {
    header.classList.remove(toggleClass);
  }
});

 document.getElementById('toggle-button').addEventListener('click', function() {
  const content = document.getElementById('content');
  if (content.classList.contains('expanded')) {
    content.classList.remove('expanded');
    this.textContent = 'Read More';
  } else {
    content.classList.add('expanded');
    this.textContent = 'Read Less';
  }
});
    
   

});



// ==============================datepicker===========================

    