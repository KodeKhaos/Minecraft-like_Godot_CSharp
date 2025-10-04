shader_type spatial;

render_mode blend_mix, depth_prepass_alpha, cull_back, unshaded;

uniform sampler2D my_texture : filter_nearest, repeat_disable;
uniform float darkness = 0.5f;

void fragment() {
	vec4 tex = texture(my_texture, UV);
	
	ALBEDO = tex.rgb * darkness;
	ALPHA = tex.a;
}
