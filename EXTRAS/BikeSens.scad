ar = 0.05;

height = 15;
width_holder = 15;

width_middle = 21;
length_middle = 135;

width_side = 150;
length_side = 15;

width_endholder_o = 50;
length_endholder_o = 40;
endholder_midoverlap = 10;

width_endholder_i = 30;
length_endholder_i = 30;

difference() {
    union() {
        translate([-width_middle/2, 0, 0])
        cube([width_middle, length_middle, height]);

        translate([-width_side/2, length_middle - length_side, 0])
        cube([width_side, length_side, height]);

        translate([-width_side/2, -5, 0])
        cube([width_side, length_side, height]);

        translate([-width_endholder_o/2, endholder_midoverlap - length_endholder_o, 0])
        cube([width_endholder_o, length_endholder_o, height]);
    }
    translate([-width_endholder_i/2, -length_endholder_i - ar, -ar])
    cube([width_endholder_i, length_endholder_i+ar, height+2*ar]);
}