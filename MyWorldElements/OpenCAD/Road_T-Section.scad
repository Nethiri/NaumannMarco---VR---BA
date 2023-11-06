//general
meter = 1;
cm = meter / 100;

module generateStreetProfile(total_width, line_width, line_left_amount, line_right_amount, sidewalk_width, sidewalk_right, sidewalk_left, bike_width, bike_left, bike_right, bike_seperator_width, bike_seperator_left_amount, bike_seperator_right_amount, curb_width, curb_left, curb_right, gen_h) {
    // Predefined values (changable)
    meter = 1;
    cm = meter / 100;
    infinite = 1 * total_width;
    sidewalk_height = gen_h * 3 / 4;
    curb_height = sidewalk_height + 1 * cm;
    road_height = curb_height - 5 * cm;
    bike_seperator_height = road_height + 3 * cm;
    bike_height = road_height;

    // Magic calculations
    line_location = 1/2 * total_width - line_width * line_left_amount;
    line_totalwidth = line_width * (line_left_amount + line_right_amount);
    bike_seperator_totalwidth = bike_seperator_width * (bike_seperator_left_amount + bike_seperator_right_amount) + line_totalwidth;
    bike_seperator_location = line_location - bike_seperator_left_amount * bike_seperator_width;
    bike_totalwidth = bike_width * (bike_left + bike_right) + bike_seperator_totalwidth;
    bike_location = bike_seperator_location - bike_width * bike_left;
    curb_totalwidth = (curb_left+curb_right) * curb_width + bike_totalwidth;
    curb_location = bike_location - curb_width * curb_left;
    sidewalk_totalwidth = (sidewalk_right + sidewalk_left) * sidewalk_width + curb_totalwidth;
    sidewalk_location = curb_location - sidewalk_width * sidewalk_left;

    difference() {
        // Generating the raw street from width
        square([total_width, gen_h]);
    
        // Driving lane
        translate([line_location, road_height, 0]) {
            square([line_totalwidth, infinite]);
        }
    
        // Bike separator
        translate([bike_seperator_location, bike_seperator_height, 0]) {
            square([bike_seperator_totalwidth, infinite]);
        }
    
        // Bike lane left / right
        difference() {
            translate([bike_location, bike_height, 0]) {
                square([bike_totalwidth, infinite]);
            }
            // Exclude the bike separator out of the two
            translate([bike_seperator_location, 0, 0]) {
                square([bike_seperator_totalwidth, infinite]);
            }
        }
    
        // Curb
        translate([curb_location, curb_height, 0]) {
            square([curb_totalwidth, infinite]);
        }

        // Sidewalk
        difference() {
            translate([sidewalk_location, sidewalk_height, 0]) {
                square([sidewalk_totalwidth, infinite]);
            }
        
            // Curb
            translate([curb_location, 0, 0]) {
                square([curb_totalwidth, infinite]);
            }
        }
        
        //remove rest
        difference() {
            square([total_width, infinite]);
            translate([sidewalk_location, 0, 0]) {
                square([sidewalk_totalwidth, infinite]);
            }
        }
    }
}



    rotate_extrude(angle=90,convexity = 10, $fn = 100)
    {
        generateStreetProfile(
            25 * meter, // total_width
            3 * meter,  // line_width
            1,          // line_left_amount
            1,          // line_right_amount
            1 * meter,  // sidewalk_width
            0,          // sidewalk_right
            1,          // sidewalk_left
            1/2 * meter, // bike_width
            1,          // bike_left
            0,          // bike_right
            1/4 * meter, // bike_seperator_width
            1,          // bike_seperator_left_amount
            0,          // bike_seperator_right_amount
            10 * cm,     // curb_width
            1,          //curb_left
            0,          //curb_right
            20 * cm     // gen_h
        );
    }
    
    translate([0,25,0])
    rotate_extrude(angle=-90,convexity = 10, $fn = 100)
    {
        generateStreetProfile(
            25 * meter, // total_width
            3 * meter,  // line_width
            1,          // line_left_amount
            1,          // line_right_amount
            1 * meter,  // sidewalk_width
            0,          // sidewalk_right
            1,          // sidewalk_left
            1/2 * meter, // bike_width
            1,          // bike_left
            0,          // bike_right
            1/4 * meter, // bike_seperator_width
            1,          // bike_seperator_left_amount
            0,          // bike_seperator_right_amount
            10 * cm,     // curb_width
            1,          //curb_left
            0,          //curb_right
            20 * cm     // gen_h
        );
    }

    translate([0,25,0])
    rotate([90,0,0])
    linear_extrude(height = 25*meter) {
        // Streight part
        generateStreetProfile(
            25 * meter, // total_width
            3 * meter,  // line_width
            1,          // line_left_amount
            1,          // line_right_amount
            1 * meter,  // sidewalk_width
            1,          // sidewalk_right
            0,          // sidewalk_left
            1/2 * meter, // bike_width
            1,          // bike_left
            1,          // bike_right
            1/4 * meter, // bike_seperator_width
            0,          // bike_seperator_left_amount
            1,          // bike_seperator_right_amount
            10 * cm,     // curb_width
            0,          //curb_left
            1,          //curb_right
            20 * cm     // gen_h
        );
    }