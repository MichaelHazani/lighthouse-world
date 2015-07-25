Spline Editor
	Spline Editor allows to create and use splines inside Unity.
	Those splines can be used as paths for objects (moving platforms, animations etc).
	Bezier, Catmull-Rom, Hermite, Kochanek-Bartels splines supported.
	
UniSpline (UniSplineComponent) - Spline whose trajectory pass trough all points.
	Supported spline types:
		Catmull-Rom, Hermite, Kochanek-Bartels
		
	Supported wrapping modes:
		Once, Loop, Repeat, PingPong.
		
	Supported arc-length reparameterization:
		None, Simple, Runge-Kutta method.
	
	Accessible trough menu:
		GameObject\Create Other\UniSpline
		
	Demonstration scene:
		Scenes\UniSplineTest
		Scenes\UniSplineCameraTest
		
	Inspector interface:
		Point count - count of points used.
		Length - length of spline.
		Type - type of spline.
		WrapMode - type of spline wrapping mode.
		Reparameterization - type of reparameterization.
		StepCount - reparameterization quality setting.
		Tension - tension parameter.
		Continuity - continuity parameter.
		Bias - bias parameter.
		Append point - append one point to spline end.
		Remove last - remove last point of spline.
		Reverse points - reverse point positions.
		Select first - selects first point.
		Select next - selects next point.
		Select previous - selects previous point.
	(depends on selected point type)
		Point type - spline point type (Corner, Smooth, Bezier, BezierCorner).
		Position - point position.
		Control - - previous segment control point position (relative to point position).
		Control + - next segment control point position (relative to point position).
		Insert before - inset point before currently selected point.
		Insert after - inset point after currently selected point.
		Remove - remove currently selected point.
	
	Programing interface:
		SplineIterator /*UniSplineComponent.Spline.*/GetIterator(); // get spline iterator
		void SplineIterator.Interate(float length); // iterate by delta
		void SplineIterator.SetOffset(float offset); // set by offset
		Vector3 SplineIterator.GetPosition(); // get current position in space
		Vector3 SplineIterator.GetPTangent(); // get tangent for current position in space
		
	
BezierSpline (BezierSplineComponent) - Spline uses configurable control points (New version of Spline)
	Supported point types:
		Corner, Smooth, Bezier, BezierCorner.
	Supported wrapping modes:
		Once, Loop, Repeat, PingPong.
	Supported arc-length reparameterization:
		None, Simple, Runge-Kutta method.
	
	Accessible trough menu:
		GameObject\Create Other\BezierSpline
		
	Demonstration scene:
		Scenes\BezierSplineTest
	
	Inspector interface:
		Point count - count of points used.
		Length - length of spline.
		WrapMode - type of spline wrapping mode.
		Reparameterization - type of reparameterization.
		StepCount - reparameterization quality setting.
		Append point - append one point to spline end.
		Remove last - remove last point of spline.
		Reverse points - reverse point positions.
		Select first - selects first point.
		Select next - selects next point.
		Select previous - selects previous point.
	(depends on selected point type)
		Point type - spline point type (Corner, Smooth, Bezier, BezierCorner).
		Position - point position.
		Control - - previous segment control point position (relative to point position).
		Control + - next segment control point position (relative to point position).
		Insert before - inset point before currently selected point.
		Insert after - inset point after currently selected point.
		Remove - remove currently selected point.
	
	Programing interface:
		SplineIterator /*BezierSplineComponent.Spline.*/GetIterator(); // get spline iterator
		void SplineIterator.Interate(float length); // iterate by delta
		void SplineIterator.SetOffset(float offset); // set by offset
		Vector3 SplineIterator.GetPosition(); // get current position in space
		Vector3 SplineIterator.GetPTangent(); // get tangent for current position in space
	
Spline (SplineComponent) - Old version of spline (initial version), currently deprecated. Please use BezierSpline instead.
	Supported point types:
		Corner, Smooth, Bezier, BezierCorner.
		
	Supported wrapping modes:
		Once, Loop, Repeat, PingPong.
		
	Accessible trough menu:
		GameObject\Create Other\Spline
	
	Demonstration scene:
		Deprecated\Scenes\SplineTest
	
	Inspector interface:
		Point count - count of points used.
		Length - length of spline.
		WrapMode - type of spline wrapping mode.
		Steps - number used to interpolate curves.
		Append point - append one point to spline end.
		Remove last - remove last point of spline.
		Reverse points - reverse point positions.
		Select first - selects first point.
		Select next - selects next point.
		Select previous - selects previous point.
	(depends on selected point type)
		Point type - spline point type (Corner, Smooth, Bezier, BezierCorner).
		Position - point position.
		Control - - previous segment control point position (relative to point position).
		Control + - next segment control point position (relative to point position).
		Insert before - inset point before currently selected point.
		Insert after - inset point after currently selected point.
		Remove - remove currently selected point.

	Programing interface:
		Vector3 GetPosition(float pos); // Get point position in space by length based position on spline.

Class description:
	BezierSplineEditor - Editor for BezierSpline component [Editor\BezierSplineEditor.cs]
	ToolsUtil - Editing helper class (hide/show tool handles) [Editor\ToolsUtil.cs]
	UniSplineEditor - Editor for UniSpline component [Editor\UniSplineEditor.cs]
	BaseSpline - Base class for UniSpline and BezierSpline [Editor\BaseSpline.cs]
	BezierSpline - BezierSpline standalone class [Editor\BezierSplineComponent.cs]
	BezierSplineComponent - BezierSpline gameobject component [Editor\BezierSplineComponent.cs]
	BezierSplineTest - BezierSpline usage test/example component [Editor\BezierSplineTest.cs]
	Ease - Various easing functions (beta version) [Editor\Ease.cs]
	SplineUtil - various common helper functions [Editor\SplineUtil.cs]
	UniSpline - UniSpline standalone class [Editor\UniSplineComponent.cs]
	UniSplineComponent - UniSpline gameobject component [Editor\UniSplineComponent.cs]
	UniSplineTest - UniSpline usage test/example component [Editor\UniSplineTest.cs]

Deprecated classes description:
	SplineEditor - Editor for Spline component [Deprecated\Editor\SplineEditor.cs]
	SplineComponent - Spline gameobject component [Deprecated\Scripts\SplineComponent.cs]
	SplineTest - Spline usage test/example component [Deprecated\Scripts\SplineTest.cs]


Developer contact:
	bojlahg@gmail.com