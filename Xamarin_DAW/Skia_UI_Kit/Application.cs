/*
 * Copyright (C) 2006 The Android Open Source Project
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace Xamarin_DAW.Skia_UI_Kit
{
    public class Application : Xamarin.Forms.BindableObject, Parent
    {
        View.AttachInfo mAttachInfo;

        public Application()
        {
            mAttachInfo = new();
            mAttachInfo.mViewRootImpl = new();
            mAttachInfo.mViewRootImpl.SetApplication(this);
        }

        private ViewRootImpl getViewRootImpl()
        {
            return mAttachInfo.mViewRootImpl;
        }

        private SkiaViewHost host;

        internal SkiaViewHost GetHost()
        {
            return host;
        }

        internal void SetHost(SkiaViewHost value)
        {
            host = value;
        }

        public void SetContentView(View view)
        {
            SetContentView(view, new View.LayoutParams(View.LayoutParams.MATCH_PARENT, View.LayoutParams.MATCH_PARENT));
        }

        public void SetContentView(View view, View.LayoutParams layoutParams)
        {
            mAttachInfo.mViewRootImpl.setContentView(view, layoutParams);
        }

        public virtual void OnCreate()
        {

        }

        public virtual void OnPause()
        {

        }

        public virtual void OnResume()
        {

        }

        public void invalidate() => host?.InvalidateSurface();

        public void INTERNAL_ERROR(string error) => host?.INTERNAL_ERROR(error);

        internal void Draw(SKCanvas canvas)
        {
            if (mAttachInfo.mViewRootImpl.hasContent())
            {
                mAttachInfo.mViewRootImpl.draw(canvas);
            }
        }

        internal void onSizeChanged(int width, int height)
        {
            mAttachInfo.mViewRootImpl.onSizeChanged(width, height);
        }

        public bool isVisible()
        {
            return host != null || host.IsVisible;
        }

        public Parent getParent()
        {
            return ((Parent)mAttachInfo.mViewRootImpl).getParent();
        }

        public void requestChildFocus(View view1, View view2)
        {
            ((Parent)mAttachInfo.mViewRootImpl).requestChildFocus(view1, view2);
        }

        public View focusSearch(View view, int direction)
        {
            return ((Parent)mAttachInfo.mViewRootImpl).focusSearch(view, direction);
        }

        public void focusableViewAvailable(View view)
        {
            ((Parent)mAttachInfo.mViewRootImpl).focusableViewAvailable(view);
        }

        public bool isLayoutRequested()
        {
            return ((Parent)mAttachInfo.mViewRootImpl).isLayoutRequested();
        }

        public void requestLayout()
        {
            ((Parent)mAttachInfo.mViewRootImpl).requestLayout();
        }

        public bool isLayoutDirectionResolved()
        {
            return ((Parent)mAttachInfo.mViewRootImpl).isLayoutDirectionResolved();
        }

        public int getLayoutDirection()
        {
            return ((Parent)mAttachInfo.mViewRootImpl).getLayoutDirection();
        }

        internal void handleAppVisibility(bool isVisible)
        {
            mAttachInfo.mViewRootImpl.handleAppVisibility(isVisible);
        }
    }
}