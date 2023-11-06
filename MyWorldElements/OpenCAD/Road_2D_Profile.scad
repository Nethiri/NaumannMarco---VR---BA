//general
meter = 1;
cm = meter / 100;


//inputs to build a road
total_width = 25*meter;
//line
line_width = 3*meter;
line_left_amount = 1;
line_right_amount = 1;
//sidewalks
sidewalk_width = 1*meter;
sidewalk_right = 1;
sidewalk_left = 1;
//bike lane
bike_width = 1/2 * meter;
bike_left = 1;
bike_right = 1;
bike_seperator_width = 1/4*meter;
bike_seperator_left_amount = 1;
bike_seperator_right_amount = 1;
//curb
curb_width = 10*cm;
curb_left = 1;
curb_right = 1;

//predefined values (changalbe)
gen_h = 20*cm;
infinite = 1*total_width;
sidewalk_height = gen_h*3/4;
curb_height = sidewalk_height + 1*cm;
road_height = curb_height - 5*cm;
bike_seperator_height = road_height + 3*cm;
bike_height = road_height;

//=========== NO TOUCHY UNLESS YOU KNOW! ===========
//magic! no touchy!
line_location = 1/2*total_width-line_width*line_left_amount;
line_totalwidth = line_width*(line_left_amount+line_right_amount);

bike_seperator_totalwidth = bike_seperator_width * (bike_seperator_left_amount + bike_seperator_right_amount) + line_totalwidth;
bike_seperator_location = line_location - bike_seperator_left_amount * bike_seperator_width;

bike_totalwidth = bike_width * (bike_left + bike_right) + bike_seperator_totalwidth;
bike_location = bike_seperator_location - bike_width * bike_left;

curb_totalwidth = (curb_left+curb_right) * curb_width + bike_totalwidth;
curb_location = bike_location - curb_width * curb_left;

sidewalk_totalwidth = (sidewalk_right + sidewalk_left) * sidewalk_width + curb_totalwidth;
sidewalk_location = curb_location - sidewalk_width * sidewalk_left;

//using inputs to generate the road



difference() {
    //generating the raw street from width
    square([total_width, gen_h]);
    
    //driving lane
    //places the line square so that middle line is in the middle of the total
    translate([line_location,road_height,0]) {
        //square representing the driving lines
        square([line_totalwidth, infinite]);
    }
    
    //bike seperator
    translate([bike_seperator_location,bike_seperator_height,0]) {
        square([bike_seperator_totalwidth,infinite]);
    }
    
    //bike lane left / right
    difference() {
        translate([bike_location,bike_height,0]) {
            square([bike_totalwidth,infinite]);
        }
        //exclude the bike seperator out of the two
        translate([bike_seperator_location,0,0]) {
            square([bike_seperator_totalwidth,infinite]);
        }
    }
    
    //curb
    translate([curb_location,curb_height,0]) {
        square([curb_totalwidth,infinite]);
    }

    //sidewalk
    difference() {
        translate([sidewalk_location, sidewalk_height, 0]) {
            square([sidewalk_totalwidth,infinite]);
        }
        
        //curb
        translate([curb_location,0,0]) {
            square([curb_totalwidth,infinite]);
        }
        
    }
}