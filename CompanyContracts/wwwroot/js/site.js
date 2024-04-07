$(document).ready(function () {
    function adjustSidebar() {
        // Get the height of the navbar
        const navbarHeight = $('.navbar').outerHeight();

        // Calculate the remaining height for the sidebar
        const sidebarHeight = $(document).height() - navbarHeight - 10;  // subtracting an extra 10px

        // Set the height and margin-top for the sidebar
        $('.sidebar').css({
            'height': sidebarHeight + 'px',
            'margin-top': '10px'
        });

        const bodyHeight = $("body").height();
        $(".sidebar").height(bodyHeight);
    }

    // Adjust the sidebar on page load
    // adjustSidebar();

    // Adjust the sidebar when the window is resized
    // $(window).on('resize', adjustSidebar);
    
    // ---------------------------------------------------------------------
});