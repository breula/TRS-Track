using CoreGraphics;
using Foundation;
using MapKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TRSTrack.Custom;
using TRSTrack.iOS.Custom;
using TRSTrack.iOS.Renders;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace TRSTrack.iOS.Renders
{
    public class CustomMapRenderer : MapRenderer
    {
        private UIView _customPinView;
        private List<CustomPin> _customPins;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                if (Control is MKMapView nativeMap)
                {
                    nativeMap.GetViewForAnnotation = null;
                    nativeMap.CalloutAccessoryControlTapped -= OnCalloutAccessoryControlTapped;
                    nativeMap.DidSelectAnnotationView -= OnDidSelectAnnotationView;
                    nativeMap.DidDeselectAnnotationView -= OnDidDeselectAnnotationView;
                }
            }

            if (e.NewElement == null) return;
            {
                var formsMap = (CustomMap)e.NewElement;
                var nativeMap = Control as MKMapView;
                _customPins = formsMap.CustomPins;

                if (nativeMap == null) return;
                nativeMap.GetViewForAnnotation = GetViewForAnnotation;
                nativeMap.CalloutAccessoryControlTapped += OnCalloutAccessoryControlTapped;
                nativeMap.DidSelectAnnotationView += OnDidSelectAnnotationView;
                nativeMap.DidDeselectAnnotationView += OnDidDeselectAnnotationView;
            }
        }

        protected override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            MKAnnotationView annotationView = null;

            var customPin = GetCustomPin(annotation as MKPointAnnotation);
            if (customPin == null)
            {
                throw new Exception("Custom pin not found");
            }

            annotationView = mapView.DequeueReusableAnnotation(customPin.Name);
            if (annotationView == null)
            {
                annotationView = new CustomMKAnnotationView(annotation, customPin.Name)
                {
                    Image = UIImage.FromFile("pin.png"),
                    CalloutOffset = new CGPoint(0, 0),
                    LeftCalloutAccessoryView = new UIImageView(UIImage.FromFile("monkey.png")),
                    RightCalloutAccessoryView = UIButton.FromType(UIButtonType.DetailDisclosure)
                };
                ((CustomMKAnnotationView)annotationView).Name = customPin.Name;
            }
            annotationView.CanShowCallout = true;

            return annotationView;
        }

        private static void OnCalloutAccessoryControlTapped(object sender, MKMapViewAccessoryTappedEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            if (!string.IsNullOrWhiteSpace(customView.Url))
            {
                UIApplication.SharedApplication.OpenUrl(new Foundation.NSUrl(customView.Url));
            }
        }

        private void OnDidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var customView = e.View as CustomMKAnnotationView;
            _customPinView = new UIView();

            if (customView != null && !customView.Name.Equals("Xamarin")) return;
            _customPinView.Frame = new CGRect(0, 0, 200, 84);
            var image = new UIImageView(new CGRect(0, 0, 200, 84)) { Image = UIImage.FromFile("xamarin.png") };
            _customPinView.AddSubview(image);
            _customPinView.Center = new CGPoint(0, -(e.View.Frame.Height + 75));
            e.View.AddSubview(_customPinView);
        }

        private void OnDidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            if (e.View.Selected) return;
            _customPinView.RemoveFromSuperview();
            _customPinView.Dispose();
            _customPinView = null;
        }

        private CustomPin GetCustomPin(MKPointAnnotation annotation)
        {
            var position = new Position(annotation.Coordinate.Latitude, annotation.Coordinate.Longitude);
            foreach (var pin in _customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }
}