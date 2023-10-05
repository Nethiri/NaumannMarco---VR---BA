int val;

const float VCC = 5.0; //supply voltage in volts
const int RC = 510; // known resistor value in ohm


void setup() {
  pinMode(A0, INPUT);
  Serial.begin(9600);
}

float convReturn(float val) {
// Vout = (VCC * 510) / (RC + 150) base forumla
  //float resistance = ((VCC * 510) /(val))-510;  
  float ret = (val * 510) / (RC + 150);
  return ret;
}

void loop() {
  long sum = 0;
  for(int i=0; i < 100; ++i) sum += (long)analogRead(A0);
  val = int(sum / 100);
  Serial.println("{\"back\": " +  String(val) + ", \"front\": "+ String(val) + "}");
  delay(1);
}

//{'resistance': 10, 'test': 10}

