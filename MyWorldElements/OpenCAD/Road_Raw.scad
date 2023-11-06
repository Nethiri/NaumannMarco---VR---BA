//constants
meter = 1;
cm = meter / 100;

general_height = 20*cm;
general_infinite = 20*meter;

//road inputs
sidewalk_height = 15*cm;
sidewalk_width = undef;
//curb inputs
curb_height = 16*cm;
curb_width = undef;
curbToPavement = 5*cm;


//road bike line defenitions
road_bike_width = 3/2 * meter;
road_line_left_bike = true;
road_line_right_bike = true;
road_seperator_right = true;
road_seperator_left = true;
road_seperator_width = 1*meter;
road_seperator_height = 1*cm;

//road line defenitions
road_line_width = 3*meter;
road_line_left_amount = 1;
road_line_right_amount = 1;

//calculated values
//road height
road_general_height = curb_height - curbToPavement;
road_general_width = undef;
road_general_center = undef;

//programm begin


module get_road(left_lane_amount, right_lane_amount, line_width) {
        road_general_center = (left_lane_amount / (left_lane_amount + right_lane_amount))*lane_width;
        road_general_width = left_lane_amount * lane_width + right_lane_amount * lane_width;
    }


difference() {
    //define the general width of the road piece, for now
    square([50*meter, general_height]);
   
    translate([10*meter,road_general_height,0]) {
        square([road_general_width, general_infinite]);
    }
}

