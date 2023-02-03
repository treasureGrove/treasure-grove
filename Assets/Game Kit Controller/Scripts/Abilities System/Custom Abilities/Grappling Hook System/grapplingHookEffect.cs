using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grapplingHookEffect : MonoBehaviour
{
	[Header ("Main Settings")]
	[Space]

	[Tooltip ("The speed of the entire effect.")]
	public float Speed = 3f;
	[Tooltip ("The speed of the spiral offset (relative to 'Speed').")]
	public float SpiralSpeed = 4f;
	[Tooltip ("The speed that the end of the line moves to the target point (relative to 'Speed')")]
	public float DistanceSpeed = 2f;
	[Tooltip ("A multiplier for the pull of gravity.")]
	public float Gravity = 0.5f;
	[Tooltip ("The number of points to be drawn by the line renderer. \nUpdates every time the effect is triggered.")]
	public int Segments = 100;
	public LayerMask layerMask;

	[Space]
	[Header ("Spiral Settings")]
	[Space]

	[Tooltip ("The strength of the spiral.")]
	public Vector2 Magnitude = Vector2.one;
	[Tooltip ("The number of 'Curve' repetitions per world-unit.")]
	public float Frequency = 0.5f;
	[Tooltip ("The amount the horizontal part of the spiral is offset along 'Curve.' \nIf 'Curve' is a sine wave, 0.25 will result in a perfect spiral.")]
	public float HorizontalOffset = 0.25f;

	[Space]
	[Header ("Noise Settings")]
	[Space]

	[Tooltip ("The strength of the noise.")]
	public float Strength = 0.5f;
	[Tooltip ("The scale of the noise samples. \nHigher = smoother.")]
	public float Scale = 0.25f;

	[Space]
	[Header ("Curves Settings")]
	[Space]

	[Tooltip ("The base curve that the points will be offset along. \nA sine wave will make it look like a grapple spiral.")]
	public AnimationCurve Curve = new AnimationCurve ();
	[Tooltip ("The strength of the spiral and noise over time.")]
	public AnimationCurve MagnitudeOverTime = new AnimationCurve ();
	[Tooltip ("The strength of the spiral and noise from the start to current end point within a 0 to 1 range.")]
	public AnimationCurve MagnitudeOverDistance = new AnimationCurve ();
	[Tooltip ("The vertical offset applied in world space to the line from the start to current end point within a 0 to 1 range.")]
	public AnimationCurve GravityOverDistance = new AnimationCurve ();
	[Tooltip ("The strength of the gravity over time.")]
	public AnimationCurve GravityOverTime = new AnimationCurve ();

	[Space]
	[Header ("Debug")]
	[Space]

	public bool grapplingHookEffectActive;
	public bool fixedUpdateActive;

	[Space]
	[Header ("Components")]
	[Space]

	public LineRenderer lineRenderer;
	public Transform mainCameraTransform;
	public Transform lineRendererOriginPositionTransform;
	public Transform lineRendererTargetPositionTransform;
	public Transform playerControllerTransform;

	float scaledTimeOffset = 0f;
	float spiralTimeOffset = 0f;
	float lastGrappleTime = 0f;
	Vector3 grapplePoint;
	RaycastHit hit;

	Vector3 currentPosition;

	public void activateGrapplingHookEffect (Vector3 raycastDirection)
	{
		if (Physics.Raycast (mainCameraTransform.position, raycastDirection, out hit, Mathf.Infinity, layerMask)) {
			lineRenderer.enabled = true;

			DoGrapple (hit.point);

			grapplingHookEffectActive = true;
		}

		Debug.DrawLine (mainCameraTransform.position, mainCameraTransform.position + raycastDirection * 1000, Color.red, 5);
	}

	public void activateGrapplingHookEffect ()
	{
		if (Physics.Raycast (mainCameraTransform.position, mainCameraTransform.forward, out hit, Mathf.Infinity, layerMask)) {
			lineRenderer.enabled = true;

			DoGrapple (hit.point);

			grapplingHookEffectActive = true;
		}
	}

	public void deactivateGrapplingHookEffect ()
	{
		grapplingHookEffectActive = false;

		lineRenderer.enabled = false;

		for (int i = 0; i < lineRenderer.positionCount; i++) {
			lineRenderer.SetPosition (i, lineRendererOriginPositionTransform.position);
		}
	}

	public void DoGrapple (Vector3 newWrapplePoint)
	{
		grapplePoint = newWrapplePoint;

		scaledTimeOffset = spiralTimeOffset = 0f;
		if (lineRenderer.positionCount != Segments)
			lineRenderer.positionCount = Segments;

		lastGrappleTime = Time.time * 10f;
	}

	public void setFixedUpdateActiveState (bool state)
	{
		fixedUpdateActive = state;
	}

	void Update ()
	{
		if (!fixedUpdateActive) {
			updateEffect ();
		}
	}

	void FixedUpdate ()
	{
		if (fixedUpdateActive) {
			updateEffect ();
		}
	}

	void updateEffect ()
	{
		if (grapplingHookEffectActive) {
			grapplePoint = lineRendererTargetPositionTransform.position;

			lineRendererOriginPositionTransform.LookAt (grapplePoint);
			var difference = grapplePoint - lineRendererOriginPositionTransform.position;
			var direction = difference.normalized;
			var distanceMultiplier = Mathf.Clamp01 (scaledTimeOffset * DistanceSpeed);
			var distance = difference.magnitude * distanceMultiplier;

			scaledTimeOffset += Speed * Time.deltaTime;

			if (distanceMultiplier < 1f) {
				spiralTimeOffset += Speed * SpiralSpeed * Time.deltaTime;
			}

			for (int i = 0; i < lineRenderer.positionCount; i++) {
				var t = (float)i / lineRenderer.positionCount;
				currentPosition = lineRendererOriginPositionTransform.position;
				var forwardOffset = direction * (t * distance);
				currentPosition += forwardOffset;

				var curveSamplePosition = forwardOffset.magnitude * Frequency - spiralTimeOffset;

				var verticalOffset = lineRendererOriginPositionTransform.up * Curve.Evaluate (curveSamplePosition);
				var horizontalOffset = lineRendererOriginPositionTransform.right * Curve.Evaluate (curveSamplePosition + HorizontalOffset);

				verticalOffset *= Magnitude.y;
				horizontalOffset *= Magnitude.x;

				var noiseSamplePosition = -t * Scale + scaledTimeOffset + lastGrappleTime;
				verticalOffset += lineRendererOriginPositionTransform.up * ((Mathf.PerlinNoise (0f, noiseSamplePosition) - 0.5f) * 2f * Strength);
				horizontalOffset += lineRendererOriginPositionTransform.right * ((Mathf.PerlinNoise (noiseSamplePosition, 0f) - 0.5f) * 2f * Strength);

				var magnitude = MagnitudeOverTime.Evaluate (scaledTimeOffset) * MagnitudeOverDistance.Evaluate (t);
				verticalOffset *= magnitude;
				horizontalOffset *= magnitude;

				currentPosition += verticalOffset;
				currentPosition += horizontalOffset;

				currentPosition += Vector3.up * (GravityOverDistance.Evaluate (t) * GravityOverTime.Evaluate (scaledTimeOffset) * Gravity);

				lineRenderer.SetPosition (i, currentPosition);
			}
		}
	}

	public void changeLineRendererOriginPositionParent (Transform newParent)
	{
		lineRendererOriginPositionTransform.SetParent (newParent);

		lineRendererOriginPositionTransform.localPosition = Vector3.zero;
	}
}
