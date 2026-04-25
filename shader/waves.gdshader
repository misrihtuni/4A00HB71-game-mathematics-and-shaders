shader_type spatial;

uniform float wave_height = 0.2;
uniform float wave_speed = 2.0;
uniform float wave_freq = 4.0;

void vertex() {
	float offset = sin(VERTEX.x * wave_freq + TIME * wave_speed);
	VERTEX.y += offset * wave_height;
}
