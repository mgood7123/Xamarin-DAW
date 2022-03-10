﻿/*
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
using Xamarin_DAW.UI.Utils;

namespace Xamarin_DAW.Skia_UI_Kit
{
    using static View.LayoutParams;


    [Xamarin.Forms.ContentProperty(nameof(Children))]
    public class View : Xamarin.Forms.BindableObject, Parent
    {
        /**
         * Always return a size of 0 for MeasureSpec values with a mode of UNSPECIFIED
         */
        const bool sUseZeroUnspecifiedMeasureSpec = false; // always false

        public View()
        {
            initView();
            InternalChildren = new();
            Children = new(InternalChildren);
        }

        /**
         * Returns {@code true} when the View is attached and the system developer setting to show
         * the layout bounds is enabled or {@code false} otherwise.
         */
        public bool isShowingLayoutBounds()
        {
            return mAttachInfo != null && mAttachInfo.mDebugLayout;
        }

        public void INTERNAL_ERROR(string error)
        {
            throw new Exception(error);

            //Parent p = mParent;
            //while (p != null)
            //{
            //    if (p is Application)
            //    {
            //        ((Application)p).INTERNAL_ERROR(error);
            //        break;
            //    }
            //    else if (p is View)
            //    {
            //        p = ((View)p).mParent;
            //    }
            //    else
            //    {
            //        break;
            //    }
            //}
        }




        public readonly System.Collections.ObjectModel.ReadOnlyCollection<View> Children;
        private List<View> InternalChildren;

        public Parent mParent { get; private set; }

        public Parent getParent()
        {
            return mParent;
        }

        public LayoutParams mLayoutParams { get; set; }

        private int l = 0, t = 0, r = 0, b = 0;

        /**
         * <summary>A MeasureSpec encapsulates the layout requirements passed from parent to child.
         * <para/>
         * Each MeasureSpec represents a requirement for either the width or the height.
         * <para/>
         * A MeasureSpec is comprised of a size and a mode. There are three possible
         * modes:
         * <para/>
         * <dl>
         * <dt>UNSPECIFIED</dt>
         * <dd>
         * <para/>
         * The parent has not imposed any constraint on the child. It can be whatever size
         * it wants.
         * </dd>
         *
         * <para/>
         * <dt>EXACTLY</dt>
         * <dd>
         * <para/>
         * The parent has determined an exact size for the child. The child is going to be
         * given those bounds regardless of how big it wants to be.
         * </dd>
         * <para/>
         * <dt>AT_MOST</dt>
         * <dd>
         * <para/>
         * The child can be as large as it wants up to the specified size.
         * </dd>
         * </dl>
         * <para/>
         * MeasureSpecs are implemented as ints to reduce object allocation. This class
         * is provided to pack and unpack the &lt;size, mode&gt; tuple into the int.</summary>
         */
        public static class MeasureSpec
        {
            internal const int MODE_SHIFT = 30;
            internal const int MODE_MASK = 0x3 << MODE_SHIFT;

            /**
             * <summary>Measure specification mode: The parent has not imposed any constraint
             * on the child. It can be whatever size it wants.</summary>
             */
            public const int UNSPECIFIED = 0 << MODE_SHIFT;

            /**
             * <summary>Measure specification mode: The parent has determined an exact size
             * for the child. The child is going to be given those bounds regardless
             * of how big it wants to be.</summary>
             */
            public const int EXACTLY = 1 << MODE_SHIFT;

            /**
             * <summary>Measure specification mode: The child can be as large as it wants up
             * to the specified size.</summary>
             */
            public const int AT_MOST = 2 << MODE_SHIFT;

            /**
             * <summary>Creates a measure specification based on the supplied size and mode.
             *
             * The mode must always be one of the following:
             * <ul>
             *  <li>{@link android.view.View.MeasureSpec#UNSPECIFIED}</li>
             *  <li>{@link android.view.View.MeasureSpec#EXACTLY}</li>
             *  <li>{@link android.view.View.MeasureSpec#AT_MOST}</li>
             * </ul>
             *
             * <p><strong>Note:</strong> On API level 17 and lower, makeMeasureSpec's
             * implementation was such that the order of arguments did not matter
             * and overflow in either value could impact the resulting MeasureSpec.
             * {@link android.widget.RelativeLayout} was affected by this bug.
             * Apps targeting API levels greater than 17 will get the fixed, more strict
             * behavior.</p></summary>
             *
             * @param size the size of the measure specification
             * @param mode the mode of the measure specification
             * @return the measure specification based on size and mode
             */
            public static int makeMeasureSpec(int size, int mode)
            {
                return (size & ~MODE_MASK) | (mode & MODE_MASK);
            }

            /**
             * Like {@link #makeMeasureSpec(int, int)}, but any spec with a mode of UNSPECIFIED
             * will automatically get a size of 0. Older apps expect this.
             *
             * @hide internal use only for compatibility with system widgets and older apps
             */
            public static int makeSafeMeasureSpec(int size, int mode)
            {
                if (sUseZeroUnspecifiedMeasureSpec && mode == UNSPECIFIED)
                {
                    return 0;
                }
                return makeMeasureSpec(size, mode);
            }

            /**
             * <summary>Creates a measure specification based on the supplied measureSpec and mode.</summary>
             *
             * The mode must always be one of the following:
             * <ul>
             *  <li>{@link android.view.View.MeasureSpec#UNSPECIFIED}</li>
             *  <li>{@link android.view.View.MeasureSpec#EXACTLY}</li>
             *  <li>{@link android.view.View.MeasureSpec#AT_MOST}</li>
             * </ul>
             * 
             * @param measureSpec the measure specification
             * @param mode the mode of the measure specification
             * @return the measure specification based on measureSpec and mode
             */
            public static int withMode(int measureSpec, int mode)
            {
                return (getSize(measureSpec) & ~MODE_MASK) | (mode & MODE_MASK);
            }

            /**
             * <summary>Creates a measure specification based on the supplied size and measureSpec.</summary>
             *
             * @param size the size of the measure specification
             * @param measureSpec the measure specification
             * @return the measure specification based on size and measureSpec
             */
            public static int withSize(int size, int measureSpec)
            {
                return (size & ~MODE_MASK) | (getMode(measureSpec) & MODE_MASK);
            }

            /**
             * <summary>Extracts the mode from the supplied measure specification.</summary>
             *
             * @param measureSpec the measure specification to extract the mode from
             * @return {@link android.view.View.MeasureSpec#UNSPECIFIED},
             *         {@link android.view.View.MeasureSpec#AT_MOST} or
             *         {@link android.view.View.MeasureSpec#EXACTLY}
             */
            public static int getMode(int measureSpec)
            {
                //noinspection ResourceType
                return (measureSpec & MODE_MASK);
            }

            /**
             * <summary>Extracts the size from the supplied measure specification.</summary>
             *
             * @param measureSpec the measure specification to extract the size from
             * @return the size in pixels defined in the supplied measure specification
             */
            public static int getSize(int measureSpec)
            {
                return (measureSpec & ~MODE_MASK);
            }

            static int adjust(int measureSpec, int delta)
            {
                int mode = getMode(measureSpec);
                int size = getSize(measureSpec);
                if (mode == UNSPECIFIED)
                {
                    // No need to adjust size for UNSPECIFIED mode.
                    return makeMeasureSpec(size, UNSPECIFIED);
                }
                size += delta;
                if (size < 0)
                {
                    Console.WriteLine("MeasureSpec.adjust: new size would be negative! (" + size +
                            ") spec: " + ToString(measureSpec) + " delta: " + delta);
                    size = 0;
                }
                return makeMeasureSpec(size, mode);
            }

            /**
             * <summary>Returns a String representation of the specified measure
             * specification.</summary>
             *
             * @param measureSpec the measure specification to convert to a String
             * @return a String with the following format: "MeasureSpec: MODE SIZE"
             */
            public static string ToString(int measureSpec)
            {
                int mode = getMode(measureSpec);
                int size = getSize(measureSpec);

                string sb = "MeasureSpec: ";
                if (mode == UNSPECIFIED)
                    sb += "UNSPECIFIED ";
                else if (mode == EXACTLY)
                    sb += "EXACTLY ";
                else if (mode == AT_MOST)
                    sb += "AT_MOST ";
                else
                    sb += mode + " ";

                sb += size;
                return sb;
            }
        }

        internal class AttachInfo
        {
            public AttachInfo()
            {
                mDebugLayout = true; // draw layout bounds

                mRecomputeGlobalAttributes = false;
                mHasWindowFocus = true;
                mApplicationScale = 1.0f;
                mScalingRequired = false;
                mHardwareAccelerated = true;
            }
            internal ViewRootImpl mViewRootImpl;
            internal bool mInTouchMode;
            internal bool mViewVisibilityChanged;
            internal bool mDebugLayout;
            internal List<View> mScrollContainers;
            internal int mWindowVisibility;
            internal int mWindowLeft;
            internal int mWindowTop;
            internal int mWindowBottom;
            internal int mWindowRight;
            internal bool mRecomputeGlobalAttributes;
            internal bool mHasWindowFocus;
            internal List<object> mPendingAnimatingRenderNodes;
            internal bool mViewScrollChanged;
            internal float mApplicationScale;
            internal bool mScalingRequired;
            internal int mDrawingTime;
            internal bool mHardwareAccelerated;

            /**
             * Used to track which View originated a requestLayout() call, used when
             * requestLayout() is called during layout.
             */
            internal View mViewRequestingLayout;

        }

        internal AttachInfo mAttachInfo;

        protected int mLayoutDirection;

        /**
         * The layout insets in pixels, that is the distance in pixels between the
         * visible edges of this view its bounds.
         */
        private Insets mLayoutInsets;

        private static float[] sDebugLines;

        /**
         * Count of how many windows this view has been attached to.
         */
        int mWindowAttachCount = 0;

        /**
         * Called when the window containing has change its visibility
         * (between {@link #GONE}, {@link #INVISIBLE}, and {@link #VISIBLE}).  Note
         * that this tells you whether or not your window is being made visible
         * to the window manager; this does <em>not</em> tell you whether or not
         * your window is obscured by other windows on the screen, even if it
         * is itself visible.
         *
         * @param visibility The new visibility of the window.
         */
        protected void onWindowVisibilityChanged(int visibility)
        {
            if (visibility == VISIBLE)
            {
                //initialAwakenScrollBars();
            }
        }

        /**
         * Returns the visibility of this view and all of its ancestors
         *
         * @return True if this view and all of its ancestors are {@link #VISIBLE}
         */
        public bool isShown()
        {
            View current = this;
            //noinspection ConstantConditions
            do
            {
                if ((current.mViewFlags & VISIBILITY_MASK) != VISIBLE)
                {
                    return false;
                }
                Parent parent = current.mParent;
                if (parent == null)
                {
                    return false; // We are not attached to the view root
                }
                if (!(parent is View))
                {
                    return true;
                }
                current = (View)parent;
            } while (current != null);

            return false;
        }

        /**
         * Set the view to be detached or not detached.
         *
         * @param detached Whether the view is detached.
         *
         * @hide
         */
        protected void setDetached(bool detached)
        {
            if (detached)
            {
                mPrivateFlags |= PFLAG_DETACHED;
            }
            else
            {
                mPrivateFlags &= ~PFLAG_DETACHED;
            }
        }

        private bool detached()
        {
            View current = this;
            //noinspection ConstantConditions
            do
            {
                if ((current.mPrivateFlags & PFLAG_DETACHED) != 0)
                {
                    return true;
                }
                Parent parent = current.mParent;
                if (parent == null)
                {
                    return false;
                }
                if (!(parent is View))
                {
                    return false;
                }
                current = (View)parent;
            } while (current != null);

            return false;
        }

        /**
         * @return true if this view and all ancestors are visible as of the last
         * {@link #onVisibilityAggregated(bool)} call.
         */
        bool isAggregatedVisible()
        {
            return (mPrivateFlags & PFLAG_AGGREGATED_VISIBLE) != 0;
        }

        /**
         * Internal dispatching method for {@link #onVisibilityAggregated}. Overridden by
         * View. Intended to only be called when {@link #isAttachedToWindow()},
         * {@link #getWindowVisibility()} is {@link #VISIBLE} and this view's parent {@link #isShown()}.
         *
         * @param isVisible true if this view's visibility to the user is uninterrupted by its
         *                  ancestors or by window visibility
         * @return true if this view is visible to the user, not counting clipping or overlapping
         */
        internal bool dispatchVisibilityAggregated(bool isVisible)
        {
            bool thisVisible = getVisibility() == VISIBLE;
            // If we're not visible but something is telling us we are, ignore it.
            if (thisVisible || !isVisible)
            {
                onVisibilityAggregated(isVisible);
            }
            return thisVisible && isVisible;
        }

        /**
         * Called when the user-visibility of this View is potentially affected by a change
         * to this view itself, an ancestor view or the window this view is attached to.
         *
         * @param isVisible true if this view and all of its ancestors are {@link #VISIBLE}
         *                  and this view's window is also visible
         */
        public void onVisibilityAggregated(bool isVisible)
        {
            // Update our internal visibility tracking so we can detect changes
            bool oldVisible = isAggregatedVisible();
            mPrivateFlags = isVisible ? (mPrivateFlags | PFLAG_AGGREGATED_VISIBLE)
                    : (mPrivateFlags & ~PFLAG_AGGREGATED_VISIBLE);
            if (isVisible && mAttachInfo != null)
            {
                //initialAwakenScrollBars();
            }

            //Drawable dr = mBackground;
            //if (dr != null && isVisible != dr.isVisible())
            //{
            //    dr.setVisible(isVisible, false);
            //}
            //Drawable hl = mDefaultFocusHighlight;
            //if (hl != null && isVisible != hl.isVisible())
            //{
            //    hl.setVisible(isVisible, false);
            //}
            //Drawable fg = mForegroundInfo != null ? mForegroundInfo.mDrawable : null;
            //if (fg != null && isVisible != fg.isVisible())
            //{
            //    fg.setVisible(isVisible, false);
            //}

            //if (isAutofillable())
            //{
            //    AutofillManager afm = getAutofillManager();

            //    if (afm != null && getAutofillViewId() > LAST_APP_AUTOFILL_ID)
            //    {
            //        if (mVisibilityChangeForAutofillHandler != null)
            //        {
            //            mVisibilityChangeForAutofillHandler.removeMessages(0);
            //        }

            //        // If the view is in the background but still part of the hierarchy this is called
            //        // with isVisible=false. Hence visibility==false requires further checks
            //        if (isVisible)
            //        {
            //            afm.notifyViewVisibilityChanged(this, true);
            //        }
            //        else
            //        {
            //            if (mVisibilityChangeForAutofillHandler == null)
            //            {
            //                mVisibilityChangeForAutofillHandler =
            //                        new VisibilityChangeForAutofillHandler(afm, this);
            //            }
            //            // Let current operation (e.g. removal of the view from the hierarchy)
            //            // finish before checking state
            //            mVisibilityChangeForAutofillHandler.obtainMessage(0, this).sendToTarget();
            //        }
            //    }
            //}

            if (isVisible != oldVisible)
            {
                //if (isAccessibilityPane())
                //{
                //    notifyViewAccessibilityStateChangedIfNeeded(isVisible
                //            ? AccessibilityEvent.CONTENT_CHANGE_TYPE_PANE_APPEARED
                //            : AccessibilityEvent.CONTENT_CHANGE_TYPE_PANE_DISAPPEARED);
                //}

                //notifyAppearedOrDisappearedForContentCaptureIfNeeded(isVisible);

                //if (!getSystemGestureExclusionRects().isEmpty())
                //{
                //    postUpdateSystemGestureExclusionRects();
                //}
            }
        }

        /**
         * Dispatch a view visibility change down the view hierarchy.
         * Views should override to route to their children.
         * @param changedView The view whose visibility changed. Could be 'this' or
         * an ancestor view.
         * @param visibility The new visibility of changedView: {@link #VISIBLE},
         * {@link #INVISIBLE} or {@link #GONE}.
         */
        protected void dispatchVisibilityChanged(View changedView,
                int visibility)
        {
            onVisibilityChanged(changedView, visibility);
        }

        /**
         * Called when the visibility of the view or an ancestor of the view has
         * changed.
         *
         * @param changedView The view whose visibility changed. May be
         *                    {@code this} or an ancestor view.
         * @param visibility The new visibility, one of {@link #VISIBLE},
         *                   {@link #INVISIBLE} or {@link #GONE}.
         */
        protected void onVisibilityChanged(View changedView, int visibility)
        {
        }

        /**
         * Return the visibility value of the least visible component passed.
         */
        int combineVisibility(int vis1, int vis2)
        {
            // This works because VISIBLE < INVISIBLE < GONE.
            return Math.Max(vis1, vis2);
        }

        /**
         * @param info the {@link android.view.View.AttachInfo} to associated with
         *        this view
         */
        internal void dispatchAttachedToWindow(AttachInfo info, int visibility)
        {
            mGroupFlags |= FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW;

            mAttachInfo = info;
            //if (mOverlay != null)
            //{
            //    mOverlay.getOverlayView().dispatchAttachedToWindow(info, visibility);
            //}
            mWindowAttachCount++;
            // We will need to evaluate the drawable state at least once.
            mPrivateFlags |= PFLAG_DRAWABLE_STATE_DIRTY;
            //if (mFloatingTreeObserver != null)
            //{
            //    info.mTreeObserver.merge(mFloatingTreeObserver);
            //    mFloatingTreeObserver = null;
            //}

            //registerPendingFrameMetricsObservers();

            if ((mPrivateFlags & PFLAG_SCROLL_CONTAINER) != 0)
            {
                mAttachInfo.mScrollContainers.Add(this);
                mPrivateFlags |= PFLAG_SCROLL_CONTAINER_ADDED;
            }
            // Transfer all pending runnables.
            //if (mRunQueue != null)
            //{
            //mRunQueue.executeActions(info.mHandler);
            //mRunQueue = null;
            //}
            //performCollectViewAttributes(mAttachInfo, visibility);
            onAttachedToWindow();

            //ListenerInfo li = mListenerInfo;
            //CopyOnWriteArrayList<OnAttachStateChangeListener> listeners =
            //        li != null ? li.mOnAttachStateChangeListeners : null;
            //if (listeners != null && listeners.size() > 0)
            //{
            //    // NOTE: because of the use of CopyOnWriteArrayList, we *must* use an iterator to
            //    // perform the dispatching. The iterator is a safe guard against listeners that
            //    // could mutate the list by calling the various add/remove methods. This prevents
            //    // the array from being modified while we iterate it.
            //    for (OnAttachStateChangeListener listener : listeners)
            //    {
            //        listener.onViewAttachedToWindow(this);
            //    }
            //}

            int vis = info.mWindowVisibility;
            if (vis != GONE)
            {
                onWindowVisibilityChanged(vis);
                if (isShown())
                {
                    // Calling onVisibilityAggregated directly here since the subtree will also
                    // receive dispatchAttachedToWindow and this same call
                    onVisibilityAggregated(vis == VISIBLE);
                }
            }

            // Send onVisibilityChanged directly instead of dispatchVisibilityChanged.
            // As all views in the subtree will already receive dispatchAttachedToWindow
            // traversing the subtree again here is not desired.
            onVisibilityChanged(this, visibility);

            if ((mPrivateFlags & PFLAG_DRAWABLE_STATE_DIRTY) != 0)
            {
                // If nobody has evaluated the drawable state yet, then do it now.
                //refreshDrawableState();
            }
            //needGlobalAttributesUpdate(false);

            //notifyEnterOrExitForAutoFillIfNeeded(true);
            //notifyAppearedOrDisappearedForContentCaptureIfNeeded(true);

            mGroupFlags &= ~FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW;

            int count = mChildrenCount;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                View child = children[i];
                child.dispatchAttachedToWindow(info,
                        combineVisibility(visibility, child.getVisibility()));
            }
            //final int transientCount = mTransientIndices == null ? 0 : mTransientIndices.size();
            //for (int i = 0; i < transientCount; ++i)
            //{
            //    View view = mTransientViews.get(i);
            //    view.dispatchAttachedToWindow(info,
            //            combineVisibility(visibility, view.getVisibility()));
            //}
        }

        // Whether any layout calls have actually been suppressed while mSuppressLayout
        // has been true. This tracks whether we need to issue a requestLayout() when
        // layout is later re-enabled.
        private bool mLayoutCalledWhileSuppressed = false;

        void dispatchDetachedFromWindow()
        {
            // If we still have a touch target, we are still in the process of
            // dispatching motion events to a child; we need to get rid of that
            // child to avoid dispatching events to it after the window is torn
            // down. To make sure we keep the child in a consistent state, we
            // first send it an ACTION_CANCEL motion event.
            //cancelAndClearTouchTargets(null);

            // Similarly, set ACTION_EXIT to all hover targets and clear them.
            //exitHoverTargets();
            //exitTooltipHoverTargets();

            // In case view is detached while transition is running
            mLayoutCalledWhileSuppressed = false;

            // Tear down our drag tracking
            //mChildrenInterestedInDrag = null;
            //mIsInterestedInDrag = false;
            //if (mCurrentDragStartEvent != null)
            //{
            //    mCurrentDragStartEvent.recycle();
            //    mCurrentDragStartEvent = null;
            //}

            int count = mChildrenCount;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                children[i].dispatchDetachedFromWindow();
            }
            //clearDisappearingChildren();
            //final int transientCount = mTransientViews == null ? 0 : mTransientIndices.size();
            //for (int i = 0; i < transientCount; ++i)
            //{
            //    View view = mTransientViews.get(i);
            //    view.dispatchDetachedFromWindow();
            //}

            AttachInfo info = mAttachInfo;
            if (info != null)
            {
                int vis = info.mWindowVisibility;
                if (vis != GONE)
                {
                    onWindowVisibilityChanged(GONE);
                    if (isShown())
                    {
                        // Invoking onVisibilityAggregated directly here since the subtree
                        // will also receive detached from window
                        onVisibilityAggregated(false);
                    }
                }
            }

            onDetachedFromWindow();
            onDetachedFromWindowInternal();

            if (info != null)
            {
                //info.mViewRootImpl.getImeFocusController().onViewDetachedFromWindow(this);
            }

            //ListenerInfo li = mListenerInfo;
            //CopyOnWriteArrayList<OnAttachStateChangeListener> listeners =
            //        li != null ? li.mOnAttachStateChangeListeners : null;
            //if (listeners != null && listeners.size() > 0)
            //{
            //    // NOTE: because of the use of CopyOnWriteArrayList, we *must* use an iterator to
            //    // perform the dispatching. The iterator is a safe guard against listeners that
            //    // could mutate the list by calling the various add/remove methods. This prevents
            //    // the array from being modified while we iterate it.
            //    for (OnAttachStateChangeListener listener : listeners)
            //    {
            //        listener.onViewDetachedFromWindow(this);
            //    }
            //}

            if ((mPrivateFlags & PFLAG_SCROLL_CONTAINER_ADDED) != 0)
            {
                mAttachInfo.mScrollContainers.Remove(this);
                mPrivateFlags &= ~PFLAG_SCROLL_CONTAINER_ADDED;
            }

            //notifyAppearedOrDisappearedForContentCaptureIfNeeded(false);

            mAttachInfo = null;
            //if (mOverlay != null)
            //{
            //    mOverlay.getOverlayView().dispatchDetachedFromWindow();
            //}

            //notifyEnterOrExitForAutoFillIfNeeded(false);

        }

        /**
         * Dispatch a window visibility change down the view hierarchy.
         * Views should override to route to their children.
         *
         * @param visibility The new visibility of the window.
         *
         * @see #onWindowVisibilityChanged(int)
         */
        public void dispatchWindowVisibilityChanged(int visibility)
        {
            onWindowVisibilityChanged(visibility);
        }

        /**
         * This is called when the view is attached to a window.  At this point it
         * has a Surface and will start drawing.  Note that this function is
         * guaranteed to be called before {@link #onDraw(android.graphics.Canvas)},
         * however it may be called any time before the first onDraw -- including
         * before or after {@link #onMeasure(int, int)}.
         *
         * @see #onDetachedFromWindow()
         */
        protected void onAttachedToWindow()
        {
            //if ((mPrivateFlags & PFLAG_REQUEST_TRANSPARENT_REGIONS) != 0)
            //{
            //mParent.requestTransparentRegion(this);
            //}

            mPrivateFlags &= ~PFLAG_IS_LAID_OUT;

            //jumpDrawablesToCurrentState();

            //AccessibilityNodeIdManager.getInstance().registerViewWithId(this, getAccessibilityViewId());
            //resetSubtreeAccessibilityStateChanged();

            // rebuild, since Outline not maintained while View is detached
            //rebuildOutline();

            //if (isFocused())
            //{
            //notifyFocusChangeToImeFocusController(true /* hasFocus */);
            //}
        }

        /**
         * Returns the width of the vertical scrollbar.
         *
         * @return The width in pixels of the vertical scrollbar or 0 if there
         *         is no vertical scrollbar.
         */
        public int getVerticalScrollbarWidth()
        {
            ScrollabilityCache cache = mScrollCache;
            if (cache != null)
            {
                ScrollBarDrawable scrollBar = cache.scrollBar;
                if (scrollBar != null)
                {
                    int size = scrollBar.getSize(true);
                    if (size <= 0)
                    {
                        size = cache.scrollBarSize;
                    }
                    return size;
                }
                return 0;
            }
            return 0;
        }

        /**
         * Returns the height of the horizontal scrollbar.
         *
         * @return The height in pixels of the horizontal scrollbar or 0 if
         *         there is no horizontal scrollbar.
         */
        protected int getHorizontalScrollbarHeight()
        {
            ScrollabilityCache cache = mScrollCache;
            if (cache != null)
            {
                ScrollBarDrawable scrollBar = cache.scrollBar;
                if (scrollBar != null)
                {
                    int size = scrollBar.getSize(false);
                    if (size <= 0)
                    {
                        size = cache.scrollBarSize;
                    }
                    return size;
                }
                return 0;
            }
            return 0;
        }

        /**
         * Position of the vertical scroll bar.
         */
        private int mVerticalScrollbarPosition = 0;

        /**
         * Position the scroll bar at the default position as determined by the system.
         */
        public const int SCROLLBAR_POSITION_DEFAULT = 0;

        /**
         * Position the scroll bar along the left edge.
         */
        public const int SCROLLBAR_POSITION_LEFT = 1;

        /**
         * Position the scroll bar along the right edge.
         */
        public const int SCROLLBAR_POSITION_RIGHT = 2;


        /**
         * @hide
         */
        protected void internalSetPadding(int left, int top, int right, int bottom)
        {
            mUserPaddingLeft = left;
            mUserPaddingRight = right;
            mUserPaddingBottom = bottom;

            int viewFlags = mViewFlags;
            bool changed = false;

            // Common case is there are no scroll bars.
            if ((viewFlags & (SCROLLBARS_VERTICAL | SCROLLBARS_HORIZONTAL)) != 0)
            {
                if ((viewFlags & SCROLLBARS_VERTICAL) != 0)
                {
                    int offset = (viewFlags & SCROLLBARS_INSET_MASK) == 0
                            ? 0 : getVerticalScrollbarWidth();
                    switch (mVerticalScrollbarPosition)
                    {
                        case SCROLLBAR_POSITION_DEFAULT:
                            if (isLayoutRtl())
                            {
                                left += offset;
                            }
                            else
                            {
                                right += offset;
                            }
                            break;
                        case SCROLLBAR_POSITION_RIGHT:
                            right += offset;
                            break;
                        case SCROLLBAR_POSITION_LEFT:
                            left += offset;
                            break;
                    }
                }
                if ((viewFlags & SCROLLBARS_HORIZONTAL) != 0)
                {
                    bottom += (viewFlags & SCROLLBARS_INSET_MASK) == 0
                            ? 0 : getHorizontalScrollbarHeight();
                }
            }

            if (mPaddingLeft != left)
            {
                changed = true;
                mPaddingLeft = left;
            }
            if (mPaddingTop != top)
            {
                changed = true;
                mPaddingTop = top;
            }
            if (mPaddingRight != right)
            {
                changed = true;
                mPaddingRight = right;
            }
            if (mPaddingBottom != bottom)
            {
                changed = true;
                mPaddingBottom = bottom;
            }

            if (changed)
            {
                requestLayout();
                //invalidateOutline();
            }
        }

        /**
         * @return true if the layout direction is inherited.
         *
         * @hide
         */
        public bool isLayoutDirectionInherited()
        {
            return (getRawLayoutDirection() == LAYOUT_DIRECTION_INHERIT);
        }

        /**
         * @return true if layout direction has been resolved.
         */
        public bool isLayoutDirectionResolved()
        {
            return (mPrivateFlags2 & PFLAG2_LAYOUT_DIRECTION_RESOLVED) == PFLAG2_LAYOUT_DIRECTION_RESOLVED;
        }

        /**
         * Return if padding has been resolved
         *
         * @hide
         */
        bool isPaddingResolved()
        {
            return (mPrivateFlags2 & PFLAG2_PADDING_RESOLVED) == PFLAG2_PADDING_RESOLVED;
        }

        /**
         * Return true if the application tag in the AndroidManifest has set "supportRtl" to true
         */
        private bool hasRtlSupport()
        {
            return false;
        }

        /**
         * Return true if we are in RTL compatibility mode (either before Jelly Bean MR1 or
         * RTL not supported)
         */
        private bool isRtlCompatibilityMode()
        {
            return !hasRtlSupport();
        }

        /**
         * @return true if RTL properties need resolution.
         *
         */
        private bool needRtlPropertiesResolution()
        {
            return (mPrivateFlags2 & ALL_RTL_PROPERTIES_RESOLVED) != ALL_RTL_PROPERTIES_RESOLVED;
        }

        /**
         * Returns the layout direction for this view.
         *
         * @return One of {@link #LAYOUT_DIRECTION_LTR},
         *   {@link #LAYOUT_DIRECTION_RTL},
         *   {@link #LAYOUT_DIRECTION_INHERIT} or
         *   {@link #LAYOUT_DIRECTION_LOCALE}.
         *
         * @attr ref android.R.styleable#View_layoutDirection
         *
         * @hide
         */
        public int getRawLayoutDirection() {
            return (mPrivateFlags2 & PFLAG2_LAYOUT_DIRECTION_MASK) >> PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT;
        }

        /**
         * Resolve and cache the layout direction. LTR is set initially. This is implicitly supposing
         * that the parent directionality can and will be resolved before its children.
         *
         * @return true if resolution has been done, false otherwise.
         *
         * @hide
         */
        public bool resolveLayoutDirection()
        {
            // Clear any previous layout direction resolution
            mPrivateFlags2 &= ~PFLAG2_LAYOUT_DIRECTION_RESOLVED_MASK;

            if (hasRtlSupport())
            {
                // Set resolved depending on layout direction
                switch ((mPrivateFlags2 & PFLAG2_LAYOUT_DIRECTION_MASK) >>
                        PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT)
                {
                    case LAYOUT_DIRECTION_INHERIT:
                        // We cannot resolve yet. LTR is by default and let the resolution happen again
                        // later to get the correct resolved value
                        if (!canResolveLayoutDirection()) return false;

                        // Parent has not yet resolved, LTR is still the default
                        if (!mParent.isLayoutDirectionResolved()) return false;

                        if (mParent.getLayoutDirection() == LAYOUT_DIRECTION_RTL)
                        {
                            mPrivateFlags2 |= PFLAG2_LAYOUT_DIRECTION_RESOLVED_RTL;
                        }
                        break;
                    case LAYOUT_DIRECTION_RTL:
                        mPrivateFlags2 |= PFLAG2_LAYOUT_DIRECTION_RESOLVED_RTL;
                        break;
                    //case LAYOUT_DIRECTION_LOCALE:
                    //    if ((LAYOUT_DIRECTION_RTL ==
                    //            TextUtils.getLayoutDirectionFromLocale(Locale.getDefault())))
                    //    {
                    //        mPrivateFlags2 |= PFLAG2_LAYOUT_DIRECTION_RESOLVED_RTL;
                    //    }
                    //    break;
                    default:
                        // Nothing to do, LTR by default
                        break;
                }
            }

            // Set to resolved
            mPrivateFlags2 |= PFLAG2_LAYOUT_DIRECTION_RESOLVED;
            return true;
        }

        /**
         * Check if layout direction resolution can be done.
         *
         * @return true if layout direction resolution can be done otherwise return false.
         */
        public bool canResolveLayoutDirection()
        {
            switch (getRawLayoutDirection())
            {
                case LAYOUT_DIRECTION_INHERIT:
                    if (mParent != null)
                    {
                        return mParent.canResolveLayoutDirection();
                    }
                    return false;

                default:
                    return true;
            }
        }

        /**
         * Resolve all RTL related properties.
         *
         * @return true if resolution of RTL properties has been done
         *
         * @hide
         */
        public bool resolveRtlPropertiesIfNeeded()
        {
            if (!needRtlPropertiesResolution()) return false;

            // Order is important here: LayoutDirection MUST be resolved first
            if (!isLayoutDirectionResolved())
            {
                resolveLayoutDirection();
                resolveLayoutParams();
            }
            // ... then we can resolve the others properties depending on the resolved LayoutDirection.
            //if (!isTextDirectionResolved())
            //{
            //    resolveTextDirection();
            //}
            //if (!isTextAlignmentResolved())
            //{
            //    resolveTextAlignment();
            //}
            // Should resolve Drawables before Padding because we need the layout direction of the
            // Drawable to correctly resolve Padding.
            //if (!areDrawablesResolved())
            //{
            //    resolveDrawables();
            //}
            if (!isPaddingResolved())
            {
                resolvePadding();
            }
            onRtlPropertiesChanged(getLayoutDirection());
            return true;
        }

        /**
         * Called when any RTL property (layout direction or text direction or text alignment) has
         * been changed.
         *
         * Subclasses need to override this method to take care of cached information that depends on the
         * resolved layout direction, or to inform child views that inherit their layout direction.
         *
         * The default implementation does nothing.
         *
         * @param layoutDirection the direction of the layout
         *
         * @see #LAYOUT_DIRECTION_LTR
         * @see #LAYOUT_DIRECTION_RTL
         */
        public void onRtlPropertiesChanged(int layoutDirection)
        {
        }

        /**
         * Reset the resolved layout direction. Layout direction will be resolved during a call to
         * {@link #onMeasure(int, int)}.
         *
         * @hide
         */
        public void resetResolvedLayoutDirection()
        {
            // Reset the current resolved bits
            mPrivateFlags2 &= ~PFLAG2_LAYOUT_DIRECTION_RESOLVED_MASK;
        }

        /**
         * Reset resolution of all RTL related properties.
         *
         * @hide
         */
        public void resetRtlProperties()
        {
            resetResolvedLayoutDirection();
            //resetResolvedTextDirection();
            //resetResolvedTextAlignment();
            resetResolvedPadding();
            //resetResolvedDrawables();
        }

        /**
         * Set the layout direction for this view. This will propagate a reset of layout direction
         * resolution to the view's children and resolve layout direction for this view.
         *
         * @param layoutDirection the layout direction to set. Should be one of:
         *
         * {@link #LAYOUT_DIRECTION_LTR},
         * {@link #LAYOUT_DIRECTION_RTL},
         * {@link #LAYOUT_DIRECTION_INHERIT},
         * {@link #LAYOUT_DIRECTION_LOCALE}.
         *
         * Resolution will be done if the value is set to LAYOUT_DIRECTION_INHERIT. The resolution
         * proceeds up the parent chain of the view to get the value. If there is no parent, then it
         * will return the default {@link #LAYOUT_DIRECTION_LTR}.
         *
         * @attr ref android.R.styleable#View_layoutDirection
         */
        public void setLayoutDirection(int layoutDirection) {
            if (getRawLayoutDirection() != layoutDirection) {
                // Reset the current layout direction and the resolved one
                mPrivateFlags2 &= ~PFLAG2_LAYOUT_DIRECTION_MASK;
                resetRtlProperties();
                // Set the new layout direction (filtered)
                mPrivateFlags2 |=
                        ((layoutDirection << PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT) & PFLAG2_LAYOUT_DIRECTION_MASK);
                // We need to resolve all RTL properties as they all depend on layout direction
                resolveRtlPropertiesIfNeeded();
                requestLayout();
                invalidate(true);
            }
        }

        /**
         * Returns the resolved layout direction for this view.
         *
         * @return {@link #LAYOUT_DIRECTION_RTL} if the layout direction is RTL or returns
         * {@link #LAYOUT_DIRECTION_LTR} if the layout direction is not RTL.
         *
         * For compatibility, this will return {@link #LAYOUT_DIRECTION_LTR} if API version
         * is lower than {@link android.os.Build.VERSION_CODES#JELLY_BEAN_MR1}.
         *
         * @attr ref android.R.styleable#View_layoutDirection
         */
        public int getLayoutDirection() {
            return ((mPrivateFlags2 & PFLAG2_LAYOUT_DIRECTION_RESOLVED_RTL) ==
                    PFLAG2_LAYOUT_DIRECTION_RESOLVED_RTL) ? LAYOUT_DIRECTION_RTL : LAYOUT_DIRECTION_LTR;
        }

        Drawable mBackground;
        /**
         * Temporary Rect currently for use in setBackground().  This will probably
         * be extended in the future to hold our own class with more than just
         * a Rect. :)
         */
        static readonly System.Threading.ThreadLocal<Rect> sThreadLocal = new (() => { return new Rect(); } );

        /**
         * Resolves padding depending on layout direction, if applicable, and
         * recomputes internal padding values to adjust for scroll bars.
         *
         * @hide
         */
        public void resolvePadding()
        {
            int resolvedLayoutDirection = getLayoutDirection();

            if (!isRtlCompatibilityMode())
            {
                // Post Jelly Bean MR1 case: we need to take the resolved layout direction into account.
                // If start / end padding are defined, they will be resolved (hence overriding) to
                // left / right or right / left depending on the resolved layout direction.
                // If start / end padding are not defined, use the left / right ones.
                if (mBackground != null && (!mLeftPaddingDefined || !mRightPaddingDefined))
                {
                    Rect padding = sThreadLocal.Value;
                    if (padding == null)
                    {
                        padding = new Rect();
                        sThreadLocal.Value = padding;
                    }
                    //mBackground.getPadding(padding);
                    if (!mLeftPaddingDefined)
                    {
                        mUserPaddingLeftInitial = padding.left;
                    }
                    if (!mRightPaddingDefined)
                    {
                        mUserPaddingRightInitial = padding.right;
                    }
                }
                switch (resolvedLayoutDirection)
                {
                    case LAYOUT_DIRECTION_RTL:
                        if (mUserPaddingStart != UNDEFINED_PADDING)
                        {
                            mUserPaddingRight = mUserPaddingStart;
                        }
                        else
                        {
                            mUserPaddingRight = mUserPaddingRightInitial;
                        }
                        if (mUserPaddingEnd != UNDEFINED_PADDING)
                        {
                            mUserPaddingLeft = mUserPaddingEnd;
                        }
                        else
                        {
                            mUserPaddingLeft = mUserPaddingLeftInitial;
                        }
                        break;
                    case LAYOUT_DIRECTION_LTR:
                    default:
                        if (mUserPaddingStart != UNDEFINED_PADDING)
                        {
                            mUserPaddingLeft = mUserPaddingStart;
                        }
                        else
                        {
                            mUserPaddingLeft = mUserPaddingLeftInitial;
                        }
                        if (mUserPaddingEnd != UNDEFINED_PADDING)
                        {
                            mUserPaddingRight = mUserPaddingEnd;
                        }
                        else
                        {
                            mUserPaddingRight = mUserPaddingRightInitial;
                        }
                        break;
                }

                mUserPaddingBottom = (mUserPaddingBottom >= 0) ? mUserPaddingBottom : mPaddingBottom;
            }

            internalSetPadding(mUserPaddingLeft, mPaddingTop, mUserPaddingRight, mUserPaddingBottom);
            onRtlPropertiesChanged(resolvedLayoutDirection);

            mPrivateFlags2 |= PFLAG2_PADDING_RESOLVED;
        }

        /**
         * Reset the resolved layout direction.
         *
         * @hide
         */
        public void resetResolvedPadding()
        {
            resetResolvedPaddingInternal();
        }

        /**
         * Used when we only want to reset *this* view's padding and not trigger overrides
         * in ViewGroup that reset children too.
         */
        void resetResolvedPaddingInternal()
        {
            mPrivateFlags2 &= ~PFLAG2_PADDING_RESOLVED;
        }

        /**
         * This is called when the view is detached from a window.  At this point it
         * no longer has a surface for drawing.
         *
         * @see #onAttachedToWindow()
         */
        protected void onDetachedFromWindow()
        {
        }

        /**
         * This is a framework-internal mirror of onDetachedFromWindow() that's called
         * after onDetachedFromWindow().
         *
         * If you override this you *MUST* call super.onDetachedFromWindowInternal()!
         * The super method should be called at the end of the overridden method to ensure
         * subclasses are destroyed first
         *
         * @hide
         */
        protected void onDetachedFromWindowInternal()
        {
            mPrivateFlags &= ~PFLAG_CANCEL_NEXT_UP_EVENT;
            mPrivateFlags &= ~PFLAG_IS_LAID_OUT;
            mPrivateFlags &= ~PFLAG_TEMPORARY_DETACH;

            removeUnsetPressCallback();
            removeLongPressCallback();
            removePerformClickCallback();
            //clearAccessibilityThrottles();
            //stopNestedScroll();

            // Anything that started animating right before detach should already
            // be in its state when re-attached.
            //jumpDrawablesToCurrentState();

            //destroyDrawingCache();

            cleanupDraw();
            //mCurrentAnimation = null;

            //if ((mViewFlags & TOOLTIP) == TOOLTIP)
            //{
            //    hideTooltip();
            //}

            //AccessibilityNodeIdManager.getInstance().unregisterViewWithId(getAccessibilityViewId());
        }

        private void cleanupDraw()
        {
            //resetDisplayList();
            if (mAttachInfo != null)
            {
                // not implemented
                // we have no way of cancelling an invalidation in a cross-platform way

                //mAttachInfo.mViewRootImpl.cancelInvalidate(this);
            }
        }

        public ViewRootImpl getViewRootImpl()
        {
            if (mAttachInfo != null)
            {
                return mAttachInfo.mViewRootImpl;
            }
            return null;
        }

        internal int mPrivateFlags;

        internal const int PFLAG_WANTS_FOCUS = 0x00000001;
        internal const int PFLAG_FOCUSED = 0x00000002;
        internal const int PFLAG_IS_LAID_OUT = 0x00000004;
        internal const int PFLAG_MEASURE_NEEDED_BEFORE_LAYOUT = 0x00000008;
        internal const int PFLAG_HAS_BOUNDS = 0x00000010;
        internal const int PFLAG_DRAWN = 0x00000020;
        internal const int PFLAG_SKIP_DRAW = 0x00000040;
        internal const int PFLAG_IS_ROOT_NAMESPACE = 0x00000080;
        internal const int PFLAG_DRAWABLE_STATE_DIRTY = 0x00000100;
        internal const int PFLAG_MEASURED_DIMENSION_SET = 0x00000200;
        internal const int PFLAG_FORCE_LAYOUT = 0x00000400;
        internal const int PFLAG_LAYOUT_REQUIRED = 0x00000800;
        internal const int PFLAG_PRESSED = 0x00001000;
        internal const int PFLAG_DRAWING_CACHE_VALID = 0x00002000;

        /**
         * Set by {@link #setScrollContainer(bool)}.
         */
        internal const int PFLAG_SCROLL_CONTAINER = 0x00004000;

        /**
         * Set by {@link #setScrollContainer(bool)}.
         */
        internal const int PFLAG_SCROLL_CONTAINER_ADDED = 0x00008000;

        /**
         * View flag indicating whether this view was invalidated (fully or partially.)
         *
         * @hide
         */
        internal const int PFLAG_DIRTY = 0x00010000;

        /**
         * Mask for {@link #PFLAG_DIRTY}.
         *
         * @hide
         */
        internal const int PFLAG_DIRTY_MASK = 0x00010000;

        /**
         * Flag indicating that the view is a root of a keyboard navigation cluster.
         *
         * @see #isKeyboardNavigationCluster()
         * @see #setKeyboardNavigationCluster(bool)
         */
        internal const int PFLAG_CLUSTER = 0x00020000;
        internal const int PFLAG_FOCUSED_BY_DEFAULT = 0x00040000;

        /**
         * Indicates a prepressed state;
         * the short time between ACTION_DOWN and recognizing
         * a 'real' press. Prepressed is used to recognize quick taps
         * even when they are shorter than ViewConfiguration.getTapTimeout().
         *
         * @hide
         */
        internal const int PFLAG_PREPRESSED = 0x00080000;

        /**
         * Indicates whether the view is temporarily detached.
         *
         * @hide
         */
        internal const int PFLAG_CANCEL_NEXT_UP_EVENT = 0x00100000;

        /** {@hide} */
        internal const int PFLAG_ACTIVATED = 0x00200000;

        /**
         * Flag indicating that the view is temporarily detached from the parent view.
         *
         * @see #onStartTemporaryDetach()
         * @see #onFinishTemporaryDetach()
         */
        internal const int PFLAG_TEMPORARY_DETACH = 0x00400000;

        /**
         * Indicates if the view is just detached.
         */
        internal const int PFLAG_DETACHED = 0x00800000;

        /**
         * The last aggregated visibility. Used to detect when it truly changes.
         */
        internal const int PFLAG_AGGREGATED_VISIBLE = 0x01000000;

        /**
         * Flag indicating whether a view failed the quickReject() check in draw(). This condition
         * is used to check whether later changes to the view's transform should invalidate the
         * view to force the quickReject test to run again.
         */
        internal const int PFLAG_VIEW_QUICK_REJECTED = 0x02000000;

        /**
         * Indicates that this view was specifically invalidated, not just dirtied because some
         * child view was invalidated. The flag is used to determine when we need to recreate
         * a view's display list (as opposed to just returning a reference to its existing
         * display list).
         *
         * @hide
         */
        internal const int PFLAG_INVALIDATED = 0x40000000;

        // for layout flags because bitshifting is involved
        internal int mPrivateFlags2;

        /** @hide */
        public enum LayoutDir
        {
                LAYOUT_DIRECTION_LTR,
                LAYOUT_DIRECTION_RTL,
                LAYOUT_DIRECTION_INHERIT,
                LAYOUT_DIRECTION_LOCALE
        }

        /** @hide */
        public enum ResolvedLayoutDir
        {
            LAYOUT_DIRECTION_LTR,
            LAYOUT_DIRECTION_RTL
        }

        /**
         * A flag to indicate that the layout direction of this view has not been defined yet.
         * @hide
         */
        public const int LAYOUT_DIRECTION_UNDEFINED = LayoutDirection.UNDEFINED;

        /**
         * Horizontal layout direction of this view is from Left to Right.
         * Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_LTR = LayoutDirection.LTR;

        /**
         * Horizontal layout direction of this view is from Right to Left.
         * Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_RTL = LayoutDirection.RTL;

        /**
         * Horizontal layout direction of this view is inherited from its parent.
         * Use with {@link #setLayoutDirection}.
         */
        public const int LAYOUT_DIRECTION_INHERIT = LayoutDirection.INHERIT;



        // not implemented

        ///**
        // * Horizontal layout direction of this view is from deduced from the default language
        // * script for the locale. Use with {@link #setLayoutDirection}.
        // */
        //public const int LAYOUT_DIRECTION_LOCALE = LayoutDirection.LOCALE;




        /**
         * Bit shift to get the horizontal layout direction. (bits after DRAG_HOVERED)
         * @hide
         */
        const int PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT = 2;

        /**
         * Mask for use with private flags indicating bits used for horizontal layout direction.
         * @hide
         */
        const int PFLAG2_LAYOUT_DIRECTION_MASK = 0x00000003 << PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT;

        /**
         * Indicates whether the view horizontal layout direction has been resolved and drawn to the
         * right-to-left direction.
         * @hide
         */
        const int PFLAG2_LAYOUT_DIRECTION_RESOLVED_RTL = 4 << PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT;

        /**
         * Indicates whether the view horizontal layout direction has been resolved.
         * @hide
         */
        const int PFLAG2_LAYOUT_DIRECTION_RESOLVED = 8 << PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT;

        /**
         * Mask for use with private flags indicating bits used for resolved horizontal layout direction.
         * @hide
         */
        const int PFLAG2_LAYOUT_DIRECTION_RESOLVED_MASK = 0x0000000C
                << PFLAG2_LAYOUT_DIRECTION_MASK_SHIFT;

        /*
         * Array of horizontal layout direction flags for mapping attribute "layoutDirection" to correct
         * flag value.
         * @hide
         */
        private readonly int[] LAYOUT_DIRECTION_FLAGS = {
                LAYOUT_DIRECTION_LTR,
                LAYOUT_DIRECTION_RTL,
                LAYOUT_DIRECTION_INHERIT
                //,LAYOUT_DIRECTION_LOCALE
        };

        /**
         * Default horizontal layout direction.
         */
        private const int LAYOUT_DIRECTION_DEFAULT = LAYOUT_DIRECTION_INHERIT;

        /**
         * Flag indicating that start/end padding has been resolved into left/right padding
         * for use in measurement, layout, drawing, etc. This is set by {@link #resolvePadding()}
         * and checked by {@link #measure(int, int)} to determine if padding needs to be resolved
         * during measurement. In some special cases this is required such as when an adapter-based
         * view measures prospective children without attaching them to a window.
         */
        internal const int PFLAG2_PADDING_RESOLVED = 0x04000000;

        /**
         * Default horizontal layout direction.
         * @hide
         */
        internal const int LAYOUT_DIRECTION_RESOLVED_DEFAULT = LAYOUT_DIRECTION_LTR;

        /**
         * Group of bits indicating that RTL properties resolution is done.
         */
        const int ALL_RTL_PROPERTIES_RESOLVED = PFLAG2_LAYOUT_DIRECTION_RESOLVED
                //| PFLAG2_TEXT_DIRECTION_RESOLVED
            //| PFLAG2_TEXT_ALIGNMENT_RESOLVED
            | PFLAG2_PADDING_RESOLVED
            //| PFLAG2_DRAWABLE_RESOLVED
            ;


        int mMeasuredWidth;
        int mMeasuredHeight;

        /**
         * The minimum height of the view. We'll try our best to have the height
         * of this view to at least this amount.
         */
        private int mMinHeight;

        /**
         * The minimum width of the view. We'll try our best to have the width
         * of this view to at least this amount.
         */
        private int mMinWidth;

        /**
         * The right padding after RTL resolution, but before taking account of scroll bars.
         *
         * @hide
         */
        protected int mUserPaddingRight;

        /**
         * The resolved bottom padding before taking account of scroll bars.
         *
         * @hide
         */
        protected int mUserPaddingBottom;

        /**
         * The left padding after RTL resolution, but before taking account of scroll bars.
         *
         * @hide
         */
        protected int mUserPaddingLeft;

        /**
         * Cache the paddingStart set by the user to append to the scrollbar's size.
         *
         */
        int mUserPaddingStart;

        /**
         * Cache the paddingEnd set by the user to append to the scrollbar's size.
         *
         */
        int mUserPaddingEnd;

        /**
         * The left padding as set by a setter method, a background's padding, or via XML property
         * resolution. This value is the padding before LTR resolution or taking account of scrollbars.
         *
         * @hide
         */
        int mUserPaddingLeftInitial;

        /**
         * The right padding as set by a setter method, a background's padding, or via XML property
         * resolution. This value is the padding before LTR resolution or taking account of scrollbars.
         *
         * @hide
         */
        int mUserPaddingRightInitial;

        /**
         * Default undefined padding
         */
        private const int UNDEFINED_PADDING = int.MinValue;

        /**
         * Cache if a left padding has been defined explicitly via padding, horizontal padding,
         * or leftPadding in XML, or by setPadding(...) or setRelativePadding(...)
         */
        private bool mLeftPaddingDefined = false;

        /**
         * Cache if a right padding has been defined explicitly via padding, horizontal padding,
         * or rightPadding in XML, or by setPadding(...) or setRelativePadding(...)
         */
        private bool mRightPaddingDefined = false;

        int mOldWidthMeasureSpec = int.MinValue;
        int mOldHeightMeasureSpec = int.MinValue;
        LongSparseLongArray mMeasureCache;

        /**
         * Bits of {@link #getMeasuredWidthAndState()} and
         * {@link #getMeasuredWidthAndState()} that provide the actual measured size.
         */
        public const int MEASURED_SIZE_MASK = 0x00ffffff;

        /**
         * Bits of {@link #getMeasuredWidthAndState()} and
         * {@link #getMeasuredWidthAndState()} that provide the additional state bits.
         */
        public const uint MEASURED_STATE_MASK = 0xff000000;

        /**
         * Bit shift of {@link #MEASURED_STATE_MASK} to get to the height bits
         * for functions that combine both width and height into a single int,
         * such as {@link #getMeasuredState()} and the childState argument of
         * {@link #resolveSizeAndState(int, int, int)}.
         */
        public const int MEASURED_HEIGHT_STATE_SHIFT = 16;

        /**
         * Bit of {@link #getMeasuredWidthAndState()} and
         * {@link #getMeasuredWidthAndState()} that indicates the measured size
         * is smaller that the space the view would like to have.
         */
        public const int MEASURED_STATE_TOO_SMALL = 0x01000000;

        /**
         * Like {@link #getMeasuredWidthAndState()}, but only returns the
         * raw width component (that is the result is masked by
         * {@link #MEASURED_SIZE_MASK}).
         *
         * @return The raw measured width of this view.
         */
        public int getMeasuredWidth()
        {
            return mMeasuredWidth & MEASURED_SIZE_MASK;
        }

        /**
         * Return the full width measurement information for this view as computed
         * by the most recent call to {@link #measure(int, int)}.  This result is a bit mask
         * as defined by {@link #MEASURED_SIZE_MASK} and {@link #MEASURED_STATE_TOO_SMALL}.
         * This should be used during measurement and layout calculations only. Use
         * {@link #getWidth()} to see how wide a view is after layout.
         *
         * @return The measured width of this view as a bit mask.
         */

        public int getMeasuredWidthAndState()
        {
            return mMeasuredWidth;
        }

        /**
         * Like {@link #getMeasuredHeightAndState()}, but only returns the
         * raw height component (that is the result is masked by
         * {@link #MEASURED_SIZE_MASK}).
         *
         * @return The raw measured height of this view.
         */
        public int getMeasuredHeight()
        {
            return mMeasuredHeight & MEASURED_SIZE_MASK;
        }

        /**
         * Return the full height measurement information for this view as computed
         * by the most recent call to {@link #measure(int, int)}.  This result is a bit mask
         * as defined by {@link #MEASURED_SIZE_MASK} and {@link #MEASURED_STATE_TOO_SMALL}.
         * This should be used during measurement and layout calculations only. Use
         * {@link #getHeight()} to see how high a view is after layout.
         *
         * @return The measured height of this view as a bit mask.
         */
        public int getMeasuredHeightAndState()
        {
            return mMeasuredHeight;
        }

        /**
         * Return only the state bits of {@link #getMeasuredWidthAndState()}
         * and {@link #getMeasuredHeightAndState()}, combined into one integer.
         * The width component is in the regular bits {@link #MEASURED_STATE_MASK}
         * and the height component is at the shifted bits
         * {@link #MEASURED_HEIGHT_STATE_SHIFT}>>{@link #MEASURED_STATE_MASK}.
         */
        public int getMeasuredState()
        {
            uint a = (uint)mMeasuredWidth & MEASURED_STATE_MASK;
            int b = mMeasuredHeight >> MEASURED_HEIGHT_STATE_SHIFT;
            uint c = MEASURED_STATE_MASK >> MEASURED_HEIGHT_STATE_SHIFT;
            return (int)(a | ((uint)b & c));
        }

        public void measure(int widthMeasureSpec, int heightMeasureSpec)
        {
            // Suppress sign extension for the low bytes
            long key = (long)widthMeasureSpec << 32 | heightMeasureSpec & 0xffffffffL;
            if (mMeasureCache == null) mMeasureCache = new(2);

            bool forceLayout = (mPrivateFlags & PFLAG_FORCE_LAYOUT) == PFLAG_FORCE_LAYOUT;

            // Optimize layout by avoiding an extra EXACTLY pass when the view is
            // already measured as the correct size. In API 23 and below, this
            // extra pass is required to make LinearLayout re-distribute weight.
            bool specChanged = widthMeasureSpec != mOldWidthMeasureSpec
                    || heightMeasureSpec != mOldHeightMeasureSpec;
            bool isSpecExactly = MeasureSpec.getMode(widthMeasureSpec) == MeasureSpec.EXACTLY
                    && MeasureSpec.getMode(heightMeasureSpec) == MeasureSpec.EXACTLY;
            bool matchesSpecSize = getMeasuredWidth() == MeasureSpec.getSize(widthMeasureSpec)
                    && getMeasuredHeight() == MeasureSpec.getSize(heightMeasureSpec);
            bool needsLayout = specChanged && (!isSpecExactly || !matchesSpecSize);

            if (forceLayout || needsLayout)
            {
                // first clears the measured dimension flag
                mPrivateFlags &= ~PFLAG_MEASURED_DIMENSION_SET;

                int cacheIndex = forceLayout ? -1 : mMeasureCache.indexOfKey(key);
                if (cacheIndex < 0)
                {
                    // measure ourselves, this should set the measured dimension flag back
                    onMeasure(widthMeasureSpec, heightMeasureSpec);
                    mPrivateFlags &= ~PFLAG_MEASURE_NEEDED_BEFORE_LAYOUT;
                }
                else
                {
                    long value = mMeasureCache.valueAt(cacheIndex);
                    // Casting a long to int drops the high 32 bits, no mask needed
                    setMeasuredDimension((int)(value >> 32), (int)value);
                    mPrivateFlags |= PFLAG_MEASURE_NEEDED_BEFORE_LAYOUT;
                }

                // flag not set, setMeasuredDimension() was not invoked, we raise
                // an exception to warn the developer
                if ((mPrivateFlags & PFLAG_MEASURED_DIMENSION_SET) != PFLAG_MEASURED_DIMENSION_SET)
                {
                    INTERNAL_ERROR("View with id " + /*getId() +*/ ": "
                            + GetType().Name + "#onMeasure() did not set the"
                            + " measured dimension by calling"
                            + " setMeasuredDimension()");
                }

                mPrivateFlags |= PFLAG_LAYOUT_REQUIRED;
            }

            mOldWidthMeasureSpec = widthMeasureSpec;
            mOldHeightMeasureSpec = heightMeasureSpec;
            mMeasureCache.put(key, ((long)mMeasuredWidth) << 32 |
                    (long)mMeasuredHeight & 0xffffffffL); // suppress sign extension
        }

        protected void setMeasuredDimension(int measuredWidth, int measuredHeight)
        {
            mMeasuredWidth = measuredWidth;
            mMeasuredHeight = measuredHeight;

            mPrivateFlags |= PFLAG_MEASURED_DIMENSION_SET;
        }

        /**
         * Merge two states as returned by {@link #getMeasuredState()}.
         * @param curState The current state as returned from a view or the result
         * of combining multiple views.
         * @param newState The new view state to combine.
         * @return Returns a new integer reflecting the combination of the two
         * states.
         */
        public static int combineMeasuredStates(int curState, int newState)
        {
            return curState | newState;
        }

        /**
         * Version of {@link #resolveSizeAndState(int, int, int)}
         * returning only the {@link #MEASURED_SIZE_MASK} bits of the result.
         */
        public static int resolveSize(int size, int measureSpec)
        {
            return resolveSizeAndState(size, measureSpec, 0) & MEASURED_SIZE_MASK;
        }

        /**
         * Utility to reconcile a desired size and state, with constraints imposed
         * by a MeasureSpec. Will take the desired size, unless a different size
         * is imposed by the constraints. The returned value is a compound integer,
         * with the resolved size in the {@link #MEASURED_SIZE_MASK} bits and
         * optionally the bit {@link #MEASURED_STATE_TOO_SMALL} set if the
         * resulting size is smaller than the size the view wants to be.
         *
         * @param size How big the view wants to be.
         * @param measureSpec Constraints imposed by the parent.
         * @param childMeasuredState Size information bit mask for the view's
         *                           children.
         * @return Size information bit mask as defined by
         *         {@link #MEASURED_SIZE_MASK} and
         *         {@link #MEASURED_STATE_TOO_SMALL}.
         */
        public static int resolveSizeAndState(int size, int measureSpec, int childMeasuredState)
        {
            int specMode = MeasureSpec.getMode(measureSpec);
            int specSize = MeasureSpec.getSize(measureSpec);
            int result;
            switch (specMode)
            {
                case MeasureSpec.AT_MOST:
                    if (specSize < size)
                    {
                        result = specSize | MEASURED_STATE_TOO_SMALL;
                    }
                    else
                    {
                        result = size;
                    }
                    break;
                case MeasureSpec.EXACTLY:
                    result = specSize;
                    break;
                case MeasureSpec.UNSPECIFIED:
                default:
                    result = size;
                    break;
            }
            return (int)((uint)result | ((uint)childMeasuredState & MEASURED_STATE_MASK));
        }

        /**
         * Utility to return a default size. Uses the supplied size if the
         * MeasureSpec imposed no constraints. Will get larger if allowed
         * by the MeasureSpec.
         *
         * @param size Default size for this view
         * @param measureSpec Constraints imposed by the parent
         * @return The size this view should be.
         */
        public static int getDefaultSize(int size, int measureSpec)
        {
            int result = size;
            int specMode = MeasureSpec.getMode(measureSpec);
            int specSize = MeasureSpec.getSize(measureSpec);

            switch (specMode)
            {
                case MeasureSpec.UNSPECIFIED:
                    result = size;
                    break;
                case MeasureSpec.AT_MOST:
                case MeasureSpec.EXACTLY:
                    result = specSize;
                    break;
            }
            return result;
        }

        /**
         * Returns the suggested minimum height that the view should use. This
         * returns the maximum of the view's minimum height
         * and the background's minimum height
         * ({@link android.graphics.drawable.Drawable#getMinimumHeight()}).
         * <p>
         * When being used in {@link #onMeasure(int, int)}, the caller should still
         * ensure the returned height is within the requirements of the parent.
         *
         * @return The suggested minimum height of the view.
         */
        protected int getSuggestedMinimumHeight()
        {
            return mMinHeight;
            //return (mBackground == null) ? mMinHeight : max(mMinHeight, mBackground.getMinimumHeight());

        }

        /**
         * Returns the suggested minimum width that the view should use. This
         * returns the maximum of the view's minimum width
         * and the background's minimum width
         *  ({@link android.graphics.drawable.Drawable#getMinimumWidth()}).
         * <p>
         * When being used in {@link #onMeasure(int, int)}, the caller should still
         * ensure the returned width is within the requirements of the parent.
         *
         * @return The suggested minimum width of the view.
         */
        protected int getSuggestedMinimumWidth()
        {
            return mMinWidth;
            //return (mBackground == null) ? mMinWidth : max(mMinWidth, mBackground.getMinimumWidth());
        }

        /**
         * Returns the minimum height of the view.
         *
         * @return the minimum height the view will try to be, in pixels
         *
         * @see #setMinimumHeight(int)
         *
         * @attr ref android.R.styleable#View_minHeight
         */
        public int getMinimumHeight()
        {
            return mMinHeight;
        }

        /**
         * Sets the minimum height of the view. It is not guaranteed the view will
         * be able to achieve this minimum height (for example, if its parent layout
         * constrains it with less available height).
         *
         * @param minHeight The minimum height the view will try to be, in pixels
         *
         * @see #getMinimumHeight()
         *
         * @attr ref android.R.styleable#View_minHeight
         */
        public void setMinimumHeight(int minHeight)
        {
            mMinHeight = minHeight;
            requestLayout();
        }

        /**
         * Returns the minimum width of the view.
         *
         * @return the minimum width the view will try to be, in pixels
         *
         * @see #setMinimumWidth(int)
         *
         * @attr ref android.R.styleable#View_minWidth
         */
        public int getMinimumWidth()
        {
            return mMinWidth;
        }

        /**
         * Sets the minimum width of the view. It is not guaranteed the view will
         * be able to achieve this minimum width (for example, if its parent layout
         * constrains it with less available width).
         *
         * @param minWidth The minimum width the view will try to be, in pixels
         *
         * @see #getMinimumWidth()
         *
         * @attr ref android.R.styleable#View_minWidth
         */
        public void setMinimumWidth(int minWidth)
        {
            mMinWidth = minWidth;
            requestLayout();

        }

        /**
         * Ask all of the children of this view to measure themselves, taking into
         * account both the MeasureSpec requirements for this view and its padding.
         * We skip children that are in the GONE state The heavy lifting is done in
         * getChildMeasureSpec.
         *
         * @param widthMeasureSpec The width requirements for this view
         * @param heightMeasureSpec The height requirements for this view
         */
        protected void measureChildren(int widthMeasureSpec, int heightMeasureSpec)
        {
            int size = mChildrenCount;
            View[] children = mChildren;
            for (int i = 0; i < size; ++i)
            {
                View child = children[i];
                if ((child.mViewFlags & VISIBILITY_MASK) != GONE)
                {
                    measureChild(child, widthMeasureSpec, heightMeasureSpec);
                }
            }
        }

        /**
         * Ask one of the children of this view to measure itself, taking into
         * account both the MeasureSpec requirements for this view and its padding.
         * The heavy lifting is done in getChildMeasureSpec.
         *
         * @param child The child to measure
         * @param parentWidthMeasureSpec The width requirements for this view
         * @param parentHeightMeasureSpec The height requirements for this view
         */
        protected void measureChild(View child, int parentWidthMeasureSpec,
                int parentHeightMeasureSpec)
        {
            LayoutParams lp = child.mLayoutParams;

            int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec,
                    mPaddingLeft + mPaddingRight, lp.width);
            int childHeightMeasureSpec = getChildMeasureSpec(parentHeightMeasureSpec,
                    mPaddingTop + mPaddingBottom, lp.height);

            child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }

        /**
         * Ask one of the children of this view to measure itself, taking into
         * account both the MeasureSpec requirements for this view and its padding
         * and margins. The child must have MarginLayoutParams The heavy lifting is
         * done in getChildMeasureSpec.
         *
         * @param child The child to measure
         * @param parentWidthMeasureSpec The width requirements for this view
         * @param widthUsed Extra space that has been used up by the parent
         *        horizontally (possibly by other children of the parent)
         * @param parentHeightMeasureSpec The height requirements for this view
         * @param heightUsed Extra space that has been used up by the parent
         *        vertically (possibly by other children of the parent)
         */
        protected void measureChildWithMargins(View child,
                int parentWidthMeasureSpec, int widthUsed,
                int parentHeightMeasureSpec, int heightUsed)
        {
            MarginLayoutParams lp = (MarginLayoutParams)child.getLayoutParams();

            int childWidthMeasureSpec = getChildMeasureSpec(parentWidthMeasureSpec,
                    mPaddingLeft + mPaddingRight + lp.leftMargin + lp.rightMargin
                            + widthUsed, lp.width);
            int childHeightMeasureSpec = getChildMeasureSpec(parentHeightMeasureSpec,
                    mPaddingTop + mPaddingBottom + lp.topMargin + lp.bottomMargin
                            + heightUsed, lp.height);

            child.measure(childWidthMeasureSpec, childHeightMeasureSpec);
        }

        /**
         * Does the hard part of measureChildren: figuring out the MeasureSpec to
         * pass to a particular child. This method figures out the right MeasureSpec
         * for one dimension (height or width) of one child view.
         *
         * The goal is to combine information from our MeasureSpec with the
         * LayoutParams of the child to get the best possible results. For example,
         * if the this view knows its size (because its MeasureSpec has a mode of
         * EXACTLY), and the child has indicated in its LayoutParams that it wants
         * to be the same size as the parent, the parent should ask the child to
         * layout given an exact size.
         *
         * @param spec The requirements for this view
         * @param padding The padding of this view for the current dimension and
         *        margins, if applicable
         * @param childDimension How big the child wants to be in the current
         *        dimension
         * @return a MeasureSpec integer for the child
         */
        public static int getChildMeasureSpec(int spec, int padding, int childDimension)
        {
            int specMode = MeasureSpec.getMode(spec);
            int specSize = MeasureSpec.getSize(spec);

            int size = Math.Max(0, specSize - padding);

            int resultSize = 0;
            int resultMode = 0;

            switch (specMode)
            {
                // Parent has imposed an exact size on us
                case MeasureSpec.EXACTLY:
                    if (childDimension >= 0)
                    {
                        resultSize = childDimension;
                        resultMode = MeasureSpec.EXACTLY;
                    }
                    else if (childDimension == LayoutParams.MATCH_PARENT)
                    {
                        // Child wants to be our size. So be it.
                        resultSize = size;
                        resultMode = MeasureSpec.EXACTLY;
                    }
                    else if (childDimension == LayoutParams.WRAP_CONTENT)
                    {
                        // Child wants to determine its own size. It can't be
                        // bigger than us.
                        resultSize = size;
                        resultMode = MeasureSpec.AT_MOST;
                    }
                    break;

                // Parent has imposed a maximum size on us
                case MeasureSpec.AT_MOST:
                    if (childDimension >= 0)
                    {
                        // Child wants a specific size... so be it
                        resultSize = childDimension;
                        resultMode = MeasureSpec.EXACTLY;
                    }
                    else if (childDimension == LayoutParams.MATCH_PARENT)
                    {
                        // Child wants to be our size, but our size is not fixed.
                        // Constrain child to not be bigger than us.
                        resultSize = size;
                        resultMode = MeasureSpec.AT_MOST;
                    }
                    else if (childDimension == LayoutParams.WRAP_CONTENT)
                    {
                        // Child wants to determine its own size. It can't be
                        // bigger than us.
                        resultSize = size;
                        resultMode = MeasureSpec.AT_MOST;
                    }
                    break;

                // Parent asked to see how big we want to be
                case MeasureSpec.UNSPECIFIED:
                    if (childDimension >= 0)
                    {
                        // Child wants a specific size... let them have it
                        resultSize = childDimension;
                        resultMode = MeasureSpec.EXACTLY;
                    }
                    else if (childDimension == LayoutParams.MATCH_PARENT)
                    {
                        // Child wants to be our size... find out how big it should
                        // be
                        resultSize = View.sUseZeroUnspecifiedMeasureSpec ? 0 : size;
                        resultMode = MeasureSpec.UNSPECIFIED;
                    }
                    else if (childDimension == LayoutParams.WRAP_CONTENT)
                    {
                        // Child wants to determine its own size.... find out how
                        // big it should be
                        resultSize = View.sUseZeroUnspecifiedMeasureSpec ? 0 : size;
                        resultMode = MeasureSpec.UNSPECIFIED;
                    }
                    break;
            }
            //noinspection ResourceType
            return MeasureSpec.makeMeasureSpec(resultSize, resultMode);
        }

        protected virtual void onMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int viewWidth = getDefaultSize(getSuggestedMinimumWidth(), widthMeasureSpec);
            int viewHeight = getDefaultSize(getSuggestedMinimumHeight(), heightMeasureSpec);

            int count = getChildCount();
            if (count > 0)
            {
                int maxHeight = 0;
                int maxWidth = 0;

                // Find out how big everyone wants to be
                measureChildren(widthMeasureSpec, heightMeasureSpec);

                // Find rightmost and bottom-most child
                for (int i = 0; i < count; i++)
                {
                    View child = getChildAt(i);
                    if (child.getVisibility() != GONE)
                    {
                        maxWidth = Math.Max(maxWidth, child.getMeasuredWidth());
                        maxHeight = Math.Max(maxHeight, child.getMeasuredHeight());
                    }
                }

                // Account for padding too
                maxWidth += mPaddingLeft + mPaddingRight;
                maxHeight += mPaddingTop + mPaddingBottom;

                // Check against minimum height and width
                viewWidth = Math.Clamp(maxWidth, 0, viewWidth);
                viewHeight = Math.Clamp(maxHeight, 0, viewHeight);
            }

            setMeasuredDimension(viewWidth, viewHeight);
        }

        public void layout(float l, float t, float r, float b)
        {
            layout(FloatToPixel(l), FloatToPixel(t), FloatToPixel(r), FloatToPixel(b));
        }

        protected int mLeft;
        protected int mTop;
        protected int mBottom;
        protected int mRight;

        /**
         * Returns true if this view has been through at least one layout since it
         * was last attached to or detached from a window.
         */
        public bool isLaidOut()
        {
            return (mPrivateFlags & PFLAG_IS_LAID_OUT) == PFLAG_IS_LAID_OUT;
        }

        /**
         * @return {@code true} if laid-out and not about to do another layout.
         */
        bool isLayoutValid()
        {
            return isLaidOut() && ((mPrivateFlags & PFLAG_FORCE_LAYOUT) == 0);
        }

        /**
         * Invoked whenever this view loses focus, either by losing window focus or by losing
         * focus within its window. This method can be used to clear any state tied to the
         * focus. For instance, if a button is held pressed with the trackball and the window
         * loses focus, this method can be used to cancel the press.
         *
         * Subclasses of View overriding this method should always call super.onFocusLost().
         *
         * @see #onFocusChanged(bool, int, android.graphics.Rect)
         * @see #onWindowFocusChanged(bool)
         *
         * @hide pending API council approval
         */
        protected void onFocusLost()
        {
            resetPressedState();
        }

        /**
         * Whether the long press's action has been invoked.  The tap's action is invoked on the
         * up event while a long press is invoked as soon as the long press duration is reached, so
         * a long press could be performed before the tap is checked, in which case the tap's action
         * should not be invoked.
         */
        private bool mHasPerformedLongPress;

        private void resetPressedState()
        {
            if ((mViewFlags & ENABLED_MASK) == DISABLED)
            {
                return;
            }

            if (isPressed())
            {
                setPressed(false);

                if (!mHasPerformedLongPress)
                {
                    removeLongPressCallback();
                }
            }
        }

        public class Runnable { };

        /**
         * <p>Removes the specified Runnable from the message queue.</p>
         *
         * @param action The Runnable to remove from the message handling queue
         *
         * @return true if this view could ask the Handler to remove the Runnable,
         *         false otherwise. When the returned value is true, the Runnable
         *         may or may not have been actually removed from the message queue
         *         (for instance, if the Runnable was not in the queue already.)
         *
         * @see #post
         * @see #postDelayed
         * @see #postOnAnimation
         * @see #postOnAnimationDelayed
         */
        public bool removeCallbacks(Runnable action)
        {
            if (action != null)
            {
                //AttachInfo attachInfo = mAttachInfo;
                //if (attachInfo != null)
                //{
                //    attachInfo.mHandler.removeCallbacks(action);
                //    attachInfo.mViewRootImpl.mChoreographer.removeCallbacks(
                //            Choreographer.CALLBACK_ANIMATION, action, null);
                //}
                //getRunQueue().removeCallbacks(action);
            }
            return true;
        }

        class CheckForLongPress : Runnable { }
        private CheckForLongPress mPendingCheckForLongPress;
        class CheckForTap : Runnable { }
        private CheckForTap mPendingCheckForTap = null;
        class PerformClick : Runnable { }
        private PerformClick mPerformClick;
        class UnsetPressedState : Runnable { }
        private UnsetPressedState mUnsetPressedState;

        /**
         * Remove the longpress detection timer.
         */
        private void removeLongPressCallback()
        {
            if (mPendingCheckForLongPress != null)
            {
                removeCallbacks(mPendingCheckForLongPress);
            }
        }

        /**
         * Return true if the long press callback is scheduled to run sometime in the future.
         * Return false if there is no scheduled long press callback at the moment.
         */
        private bool hasPendingLongPressCallback()
        {
            if (mPendingCheckForLongPress == null)
            {
                return false;
            }
            //AttachInfo attachInfo = mAttachInfo;
            //if (attachInfo == null)
            //{
            return false;
            //}
            //return attachInfo.mHandler.hasCallbacks(mPendingCheckForLongPress);
        }

        /**
          * Remove the pending click action
          */
        private void removePerformClickCallback()
        {
            if (mPerformClick != null)
            {
                removeCallbacks(mPerformClick);
            }
        }

        /**
         * Remove the prepress detection timer.
         */
        private void removeUnsetPressCallback()
        {
            if ((mPrivateFlags & PFLAG_PRESSED) != 0 && mUnsetPressedState != null)
            {
                setPressed(false);
                removeCallbacks(mUnsetPressedState);
            }
        }

        /**
         * Remove the tap detection timer.
         */
        private void removeTapCallback()
        {
            if (mPendingCheckForTap != null)
            {
                mPrivateFlags &= ~PFLAG_PREPRESSED;
                removeCallbacks(mPendingCheckForTap);
            }
        }

        /**
         * Cancels a pending long press.  Your subclass can use this if you
         * want the context menu to come up if the user presses and holds
         * at the same place, but you don't want it to come up if they press
         * and then move around enough to cause scrolling.
         */
        public void cancelLongPress()
        {
            removeLongPressCallback();

            /*
             * The prepressed state handled by the tap callback is a display
             * construct, but the tap callback will post a long press callback
             * less its own timeout. Remove it here.
             */
            removeTapCallback();
        }

        /**
         * Returns true if this view has focus
         *
         * @return True if this view has focus, false otherwise.
         */
        public bool isFocused()
        {
            return (mPrivateFlags & PFLAG_FOCUSED) != 0;
        }

        /**
         * Called internally by the view system when a new view is getting focus.
         * This is what clears the old focus.
         * <p>
         * <b>NOTE:</b> The parent view's focused child must be updated manually
         * after calling this method. Otherwise, the view hierarchy may be left in
         * an inconstent state.
         */
        void unFocus(View focused)
        {
            if (DBG)
            {
                System.Console.WriteLine(this + " unFocus()");
            }

            if (mFocused == null)
            {
                clearFocusInternal(focused, false, false);
            }
            else
            {
                mFocused.unFocus(focused);
                mFocused = null;
            }
        }

        /**
         * Returns true if this view has focus itself, or is the ancestor of the
         * view that has focus.
         *
         * @return True if this view has or contains focus, false otherwise.
         */
        public bool hasFocus()
        {
            return (mPrivateFlags & PFLAG_FOCUSED) != 0 || mFocused != null;
        }

        /**
         * Find the view in the hierarchy rooted at this view that currently has
         * focus.
         *
         * @return The view that currently has focus, or null if no focused view can
         *         be found.
         */
        public View findFocus()
        {
            if (isFocused())
            {
                return this;
            }

            if (mFocused != null)
            {
                return mFocused.findFocus();
            }
            return null;
        }

        private bool hasSize()
        {
            return (mBottom > mTop) && (mRight > mLeft);
        }

        /**
         * This view does not want keystrokes.
         * <p>
         * Use with {@link #setFocusable(int)} and <a href="#attr_android:focusable">{@code
         * android:focusable}.
         */
        public const int NOT_FOCUSABLE = 0x00000000;

        /**
         * This view wants keystrokes.
         * <p>
         * Use with {@link #setFocusable(int)} and <a href="#attr_android:focusable">{@code
         * android:focusable}.
         */
        public const int FOCUSABLE = 0x00000001;

        /**
         * This view determines focusability automatically. This is the default.
         * <p>
         * Use with {@link #setFocusable(int)} and <a href="#attr_android:focusable">{@code
         * android:focusable}.
         */
        public const int FOCUSABLE_AUTO = 0x00000010;

        /**
         * Mask for use with setFlags indicating bits used for focus.
         */
        internal const int FOCUSABLE_MASK = 0x00000011;

        /**
         * This view is visible.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int VISIBLE = 0x00000000;

        /**
         * This view is invisible, but it still takes up space for layout purposes.
         * Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int INVISIBLE = 0x00000004;

        /**
         * This view is invisible, and it doesn't take any space for layout
         * purposes. Use with {@link #setVisibility} and <a href="#attr_android:visibility">{@code
         * android:visibility}.
         */
        public const int GONE = 0x00000008;

        /**
         * Mask for use with setFlags indicating bits used for visibility.
         * {@hide}
         */
        internal const int VISIBILITY_MASK = 0x0000000C;

        private static readonly int[] VISIBILITY_FLAGS = { VISIBLE, INVISIBLE, GONE };

        /**
         * This view is enabled. Interpretation varies by subclass.
         * Use with ENABLED_MASK when calling setFlags.
         * {@hide}
         */
        internal const int ENABLED = 0x00000000;

        /**
         * This view is disabled. Interpretation varies by subclass.
         * Use with ENABLED_MASK when calling setFlags.
         * {@hide}
         */
        internal const int DISABLED = 0x00000020;

        /**
         * Mask for use with setFlags indicating bits used for indicating whether
         * this view is enabled
         * {@hide}
         */
        internal const int ENABLED_MASK = 0x00000020;

        /**
         * <p>Indicates this view can take / keep focus when int touch mode.</p>
         * {@hide}
         */
        internal const int FOCUSABLE_IN_TOUCH_MODE = 0x00040000;

        /**
         * <p>
         * Indicates this view can be context clicked. When context clickable, a View reacts to a
         * context click (e.g. a primary stylus button press or right mouse click) by notifying the
         * OnContextClickListener.
         * </p>
         * {@hide}
         */
        internal const int CONTEXT_CLICKABLE = 0x00800000;

        /**
         * The scrollbar style to display the scrollbars inside the content area,
         * without increasing the padding. The scrollbars will be overlaid with
         * translucency on the view's content.
         */
        public const int SCROLLBARS_INSIDE_OVERLAY = 0;

        /**
         * The scrollbar style to display the scrollbars inside the padded area,
         * increasing the padding of the view. The scrollbars will not overlap the
         * content area of the view.
         */
        public const int SCROLLBARS_INSIDE_INSET = 0x01000000;

        /**
         * The scrollbar style to display the scrollbars at the edge of the view,
         * without increasing the padding. The scrollbars will be overlaid with
         * translucency.
         */
        public const int SCROLLBARS_OUTSIDE_OVERLAY = 0x02000000;

        /**
         * The scrollbar style to display the scrollbars at the edge of the view,
         * increasing the padding of the view. The scrollbars will only overlap the
         * background, if any.
         */
        public const int SCROLLBARS_OUTSIDE_INSET = 0x03000000;

        /**
         * Mask to check if the scrollbar style is overlay or inset.
         * {@hide}
         */
        const int SCROLLBARS_INSET_MASK = 0x01000000;

        /**
         * Mask to check if the scrollbar style is inside or outside.
         * {@hide}
         */
        const int SCROLLBARS_OUTSIDE_MASK = 0x02000000;

        /**
         * Mask for scrollbar style.
         * {@hide}
         */
        const int SCROLLBARS_STYLE_MASK = 0x03000000;

        internal int mViewFlags;

        private bool canTakeFocus()
        {
            return ((mViewFlags & VISIBILITY_MASK) == VISIBLE)
                    && ((mViewFlags & FOCUSABLE) == FOCUSABLE)
                    && ((mViewFlags & ENABLED_MASK) == ENABLED)
                    && (!isLayoutValid() || hasSize());
        }

        // The view contained within this View that has or contains focus.
        private View mFocused;
        // The view contained within this View (excluding nested keyboard navigation clusters)
        // that is or contains a default-focus view.
        private View mDefaultFocus;
        // The last child of this View which held focus within the current cluster
        View mFocusedInCluster;

        bool rootViewRequestFocus()
        {
            return false;
        }

        /**
         * Sets the pressed state for this view.
         *
         * @see #isClickable()
         * @see #setClickable(bool)
         *
         * @param pressed Pass true to set the View's internal state to "pressed", or false to reverts
         *        the View's internal state from a previously set "pressed" state.
         */
        public void setPressed(bool pressed)
        {
            bool needsRefresh = pressed != ((mPrivateFlags & PFLAG_PRESSED) == PFLAG_PRESSED);

            if (pressed)
            {
                mPrivateFlags |= PFLAG_PRESSED;
            }
            else
            {
                mPrivateFlags &= ~PFLAG_PRESSED;
            }

            if (needsRefresh)
            {
                //refreshDrawableState();
            }
            dispatchSetPressed(pressed);
        }

        /**
         * Dispatch setPressed to all of this View's children.
         *
         * @see #setPressed(bool)
         *
         * @param pressed The new pressed state
         */
        protected void dispatchSetPressed(bool pressed)
        {
        }

        /**
         * Indicates whether the view is currently in pressed state. Unless
         * {@link #setPressed(bool)} is explicitly called, only clickable views can enter
         * the pressed state.
         *
         * @see #setPressed(bool)
         * @see #isClickable()
         * @see #setClickable(bool)
         *
         * @return true if the view is currently pressed, false otherwise
         */
        public bool isPressed()
        {
            return (mPrivateFlags & PFLAG_PRESSED) == PFLAG_PRESSED;
        }

        /**
         * Called by the view system when the focus state of this view changes.
         * When the focus change event is caused by directional navigation, direction
         * and previouslyFocusedRect provide insight into where the focus is coming from.
         * When overriding, be sure to call up through to the super class so that
         * the standard focus handling will occur.
         *
         * @param gainFocus True if the View has focus; false otherwise.
         * @param direction The direction focus has moved when requestFocus()
         *                  is called to give this view focus. Values are
         *                  {@link #FOCUS_UP}, {@link #FOCUS_DOWN}, {@link #FOCUS_LEFT},
         *                  {@link #FOCUS_RIGHT}, {@link #FOCUS_FORWARD}, or {@link #FOCUS_BACKWARD}.
         *                  It may not always apply, in which case use the default.
         * @param previouslyFocusedRect The rectangle, in this view's coordinate
         *        system, of the previously focused view.  If applicable, this will be
         *        passed in as finer grained information about where the focus is coming
         *        from (in addition to direction).  Will be <code>null</code> otherwise.
         */
        protected void onFocusChanged(bool gainFocus, int direction, Rect previouslyFocusedRect)
        {
            if (!gainFocus)
            {
                if (isPressed())
                {
                    setPressed(false);
                }
                //if (hasWindowFocus())
                //{
                //notifyFocusChangeToImeFocusController(false /* hasFocus */);
                //}
                onFocusLost();
            }
            //else if (hasWindowFocus())
            //{
            //notifyFocusChangeToImeFocusController(true /* hasFocus */);
            //}

            invalidate(true);
            //ListenerInfo li = mListenerInfo;
            //if (li != null && li.mOnFocusChangeListener != null)
            //{
            //    li.mOnFocusChangeListener.onFocusChange(this, gainFocus);
            //}

            //if (mAttachInfo != null)
            //{
            //    mAttachInfo.mKeyDispatchState.reset(this);
            //}

            if (mParent != null)
            {
                mParent.onDescendantUnbufferedRequested();
            }
        }

        private bool hasParentWantsFocus()
        {
            Parent parent = mParent;
            while (parent is View)
            {
                View pv = (View)parent;
                if ((pv.mPrivateFlags & PFLAG_WANTS_FOCUS) != 0)
                {
                    return true;
                }
                parent = pv.mParent;
            }
            return false;
        }

        /**
         * Use with {@link #focusSearch(int)}. Move focus to the previous selectable
         * item.
         */
        public const int FOCUS_BACKWARD = 0x00000001;

        /**
         * Use with {@link #focusSearch(int)}. Move focus to the next selectable
         * item.
         */
        public const int FOCUS_FORWARD = 0x00000002;

        /**
         * Use with {@link #focusSearch(int)}. Move focus to the left.
         */
        public const int FOCUS_LEFT = 0x00000011;

        /**
         * Use with {@link #focusSearch(int)}. Move focus up.
         */
        public const int FOCUS_UP = 0x00000021;

        /**
         * Use with {@link #focusSearch(int)}. Move focus to the right.
         */
        public const int FOCUS_RIGHT = 0x00000042;

        /**
         * Use with {@link #focusSearch(int)}. Move focus down.
         */
        public const int FOCUS_DOWN = 0x00000082;

        /**
         * Gets the id of the view to use when the next focus is {@link #FOCUS_LEFT}.
         * @return The next focus ID, or {@link #NO_ID} if the framework should decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusLeft
         */
        public int getNextFocusLeftId()
        {
            return mNextFocusLeftId;
        }

        /**
         * Sets the id of the view to use when the next focus is {@link #FOCUS_LEFT}.
         * @param nextFocusLeftId The next focus ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusLeft
         */
        public void setNextFocusLeftId(int nextFocusLeftId)
        {
            mNextFocusLeftId = nextFocusLeftId;
        }

        /**
         * Gets the id of the view to use when the next focus is {@link #FOCUS_RIGHT}.
         * @return The next focus ID, or {@link #NO_ID} if the framework should decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusRight
         */
        public int getNextFocusRightId()
        {
            return mNextFocusRightId;
        }

        /**
         * Sets the id of the view to use when the next focus is {@link #FOCUS_RIGHT}.
         * @param nextFocusRightId The next focus ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusRight
         */
        public void setNextFocusRightId(int nextFocusRightId)
        {
            mNextFocusRightId = nextFocusRightId;
        }

        /**
         * Gets the id of the view to use when the next focus is {@link #FOCUS_UP}.
         * @return The next focus ID, or {@link #NO_ID} if the framework should decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusUp
         */
        public int getNextFocusUpId()
        {
            return mNextFocusUpId;
        }

        /**
         * Sets the id of the view to use when the next focus is {@link #FOCUS_UP}.
         * @param nextFocusUpId The next focus ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusUp
         */
        public void setNextFocusUpId(int nextFocusUpId)
        {
            mNextFocusUpId = nextFocusUpId;
        }

        /**
         * Gets the id of the view to use when the next focus is {@link #FOCUS_DOWN}.
         * @return The next focus ID, or {@link #NO_ID} if the framework should decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusDown
         */
        public int getNextFocusDownId()
        {
            return mNextFocusDownId;
        }

        /**
         * Sets the id of the view to use when the next focus is {@link #FOCUS_DOWN}.
         * @param nextFocusDownId The next focus ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusDown
         */
        public void setNextFocusDownId(int nextFocusDownId)
        {
            mNextFocusDownId = nextFocusDownId;
        }

        /**
         * Gets the id of the view to use when the next focus is {@link #FOCUS_FORWARD}.
         * @return The next focus ID, or {@link #NO_ID} if the framework should decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusForward
         */
        public int getNextFocusForwardId()
        {
            return mNextFocusForwardId;
        }

        /**
         * Sets the id of the view to use when the next focus is {@link #FOCUS_FORWARD}.
         * @param nextFocusForwardId The next focus ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextFocusForward
         */
        public void setNextFocusForwardId(int nextFocusForwardId)
        {
            mNextFocusForwardId = nextFocusForwardId;
        }

        /**
         * Gets the id of the root of the next keyboard navigation cluster.
         * @return The next keyboard navigation cluster ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextClusterForward
         */
        public int getNextClusterForwardId()
        {
            return mNextClusterForwardId;
        }

        /**
         * Sets the id of the view to use as the root of the next keyboard navigation cluster.
         * @param nextClusterForwardId The next cluster ID, or {@link #NO_ID} if the framework should
         * decide automatically.
         *
         * @attr ref android.R.styleable#View_nextClusterForward
         */
        public void setNextClusterForwardId(int nextClusterForwardId)
        {
            mNextClusterForwardId = nextClusterForwardId;
        }

        /**
         * If a user manually specified the next view id for a particular direction,
         * use the root to look up the view.
         * @param root The root view of the hierarchy containing this view.
         * @param direction One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, FOCUS_RIGHT, FOCUS_FORWARD,
         * or FOCUS_BACKWARD.
         * @return The user specified next view, or null if there is none.
         */
        public View findUserSetNextFocus(View root, int direction)
        {
            switch (direction)
            {
                case FOCUS_LEFT:
                    if (mNextFocusLeftId == View.NO_ID) return null;
                    return findViewInsideOutShouldExist(root, mNextFocusLeftId);
                case FOCUS_RIGHT:
                    if (mNextFocusRightId == View.NO_ID) return null;
                    return findViewInsideOutShouldExist(root, mNextFocusRightId);
                case FOCUS_UP:
                    if (mNextFocusUpId == View.NO_ID) return null;
                    return findViewInsideOutShouldExist(root, mNextFocusUpId);
                case FOCUS_DOWN:
                    if (mNextFocusDownId == View.NO_ID) return null;
                    return findViewInsideOutShouldExist(root, mNextFocusDownId);
                case FOCUS_FORWARD:
                    if (mNextFocusForwardId == View.NO_ID) return null;
                    return findViewInsideOutShouldExist(root, mNextFocusForwardId);
                case FOCUS_BACKWARD:
                    {
                        if (mID == NO_ID) return null;
                        View rootView = root;
                        View startView = this;
                        // Since we have forward links but no backward links, we need to find the view that
                        // forward links to this view. We can't just find the view with the specified ID
                        // because view IDs need not be unique throughout the tree.
                        return root.findViewByPredicateInsideOut<View>(
                            startView,
                            t => t.findViewInsideOutShouldExist(rootView, t, t.mNextFocusForwardId)
                                    == startView);
                    }
            }
            return null;
        }

        /**
         * If a user manually specified the next keyboard-navigation cluster for a particular direction,
         * use the root to look up the view.
         *
         * @param root the root view of the hierarchy containing this view
         * @param direction {@link #FOCUS_FORWARD} or {@link #FOCUS_BACKWARD}
         * @return the user-specified next cluster, or {@code null} if there is none
         */
        public View findUserSetNextKeyboardNavigationCluster(View root, int direction)
        {
            switch (direction)
            {
                case FOCUS_FORWARD:
                    if (mNextClusterForwardId == View.NO_ID) return null;
                    return findViewInsideOutShouldExist(root, mNextClusterForwardId);
                case FOCUS_BACKWARD:
                    {
                        if (mID == View.NO_ID) return null;
                        int id = mID;
                        return root.findViewByPredicateInsideOut<View>(this,
                                t => t.mNextClusterForwardId == id);
                    }
            }
            return null;
        }

        /**
         * View flag indicating whether {@link #addFocusables(ArrayList, int, int)}
         * should add all focusable Views regardless if they are focusable in touch mode.
         */
        public const int FOCUSABLES_ALL = 0x00000000;

        /**
         * View flag indicating whether {@link #addFocusables(ArrayList, int, int)}
         * should add only Views focusable in touch mode.
         */
        public const int FOCUSABLES_TOUCH_MODE = 0x00000001;

        /**
         * When a view is focusable, it may not want to take focus when in touch mode.
         * For example, a button would like focus when the user is navigating via a D-pad
         * so that the user can click on it, but once the user starts touching the screen,
         * the button shouldn't take focus
         * @return Whether the view is focusable in touch mode.
         * @attr ref android.R.styleable#View_focusableInTouchMode
         */
        public bool isFocusableInTouchMode()
        {
            return FOCUSABLE_IN_TOUCH_MODE == (mViewFlags & FOCUSABLE_IN_TOUCH_MODE);
        }

        /**
         * Add any focusable views that are descendants of this view (possibly
         * including this view if it is focusable itself) to views.  If we are in touch mode,
         * only add views that are also focusable in touch mode.
         *
         * @param views Focusable views found so far
         * @param direction The direction of the focus
         */
        public void addFocusables(List<View> views, int direction)
        {
            addFocusables(views, direction, isInTouchMode() ? FOCUSABLES_TOUCH_MODE : FOCUSABLES_ALL);
        }

        /**
         * Adds any focusable views that are descendants of this view (possibly
         * including this view if it is focusable itself) to views. This method
         * adds all focusable views regardless if we are in touch mode or
         * only views focusable in touch mode if we are in touch mode or
         * only views that can take accessibility focus if accessibility is enabled
         * depending on the focusable mode parameter.
         *
         * @param views Focusable views found so far or null if all we are interested is
         *        the number of focusables.
         * @param direction The direction of the focus.
         * @param focusableMode The type of focusables to be added.
         *
         * @see #FOCUSABLES_ALL
         * @see #FOCUSABLES_TOUCH_MODE
         */
        public void addFocusables(List<View> views, int direction, int focusableMode)
        {
            if (views == null)
            {
                return;
            }
            if (!canTakeFocus())
            {
                return;
            }
            if ((focusableMode & FOCUSABLES_TOUCH_MODE) == FOCUSABLES_TOUCH_MODE
                    && !isFocusableInTouchMode())
            {
                return;
            }
            views.Add(this);
        }

        /**
         * Adds any keyboard navigation cluster roots that are descendants of this view (possibly
         * including this view if it is a cluster root itself) to views.
         *
         * @param views Keyboard navigation cluster roots found so far
         * @param direction Direction to look
         */
        public void addKeyboardNavigationClusters(
                List<View> views,
                int direction)
        {
            if (!isKeyboardNavigationCluster())
            {
                return;
            }
            if (!hasFocusable())
            {
                return;
            }
            views.Add(this);
        }

        /**
         * Returns the visibility status for this view.
         *
         * @return One of {@link #VISIBLE}, {@link #INVISIBLE}, or {@link #GONE}.
         * @attr ref android.R.styleable#View_visibility
         */
        public int getVisibility()
        {
            return mViewFlags & VISIBILITY_MASK;
        }

        /**
         * Set the visibility state of this view.
         *
         * @param visibility One of {@link #VISIBLE}, {@link #INVISIBLE}, or {@link #GONE}.
         * @attr ref android.R.styleable#View_visibility
         */
        public void setVisibility(int visibility)
        {
            setFlags(visibility, VISIBILITY_MASK);
        }

        /**
         * <p>This view doesn't show fading edges.</p>
         * {@hide}
         */
        internal const int FADING_EDGE_NONE = 0x00000000;

        /**
         * <p>This view shows horizontal fading edges.</p>
         * {@hide}
         */
        internal const int FADING_EDGE_HORIZONTAL = 0x00001000;

        /**
         * <p>This view shows vertical fading edges.</p>
         * {@hide}
         */
        internal const int FADING_EDGE_VERTICAL = 0x00002000;

        /**
         * <p>Mask for use with setFlags indicating bits used for indicating which
         * fading edges are enabled.</p>
         * {@hide}
         */
        internal const int FADING_EDGE_MASK = 0x00003000;

        /**
         * <p>Indicates this view can be clicked. When clickable, a View reacts
         * to clicks by notifying the OnClickListener.<p>
         * {@hide}
         */
        internal const int CLICKABLE = 0x00004000;

        /**
         * <p>
         * Indicates this view can be long clicked. When long clickable, a View
         * reacts to long clicks by notifying the OnLongClickListener or showing a
         * context menu.
         * </p>
         * {@hide}
         */
        internal const int LONG_CLICKABLE = 0x00200000;

        /**
         * Called when this view wants to give up focus. If focus is cleared
         * {@link #onFocusChanged(bool, int, android.graphics.Rect)} is called.
         * <p>
         * <strong>Note:</strong> When not in touch-mode, the framework will try to give focus
         * to the first focusable View from the top after focus is cleared. Hence, if this
         * View is the first from the top that can take focus, then all callbacks
         * related to clearing focus will be invoked after which the framework will
         * give focus to this view.
         * </p>
         */
        public void clearFocus()
        {
            if (DBG)
            {
                System.Console.WriteLine(this + " clearFocus()");
            }

            bool refocus = !isInTouchMode();
            clearFocusInternal(null, true, refocus);
        }

        public void focusableViewAvailable(View v)
        {
            if (mParent != null
                    // shortcut: don't report a new focusable view if we block our descendants from
                    // getting focus or if we're not visible
                    && (getDescendantFocusability() != FOCUS_BLOCK_DESCENDANTS)
                    && ((mViewFlags & VISIBILITY_MASK) == VISIBLE)
                    && (isFocusableInTouchMode() || !shouldBlockFocusForTouchscreen())
                    // shortcut: don't report a new focusable view if we already are focused
                    // (and we don't prefer our descendants)
                    //
                    // note: knowing that mFocused is non-null is not a good enough reason
                    // to break the traversal since in that case we'd actually have to find
                    // the focused view and make sure it wasn't FOCUS_AFTER_DESCENDANTS and
                    // an ancestor of v; this will get checked for at ViewAncestor
                    && !(isFocused() && getDescendantFocusability() != FOCUS_AFTER_DESCENDANTS))
            {
                mParent.focusableViewAvailable(v);
            }
        }

        /**
         * <p>Finds the topmost view in the current view hierarchy.</p>
         *
         * @return the topmost view containing this view
         */
        public View getRootView()
        {
            if (mAttachInfo != null)
            {
                //View v = mAttachInfo.mRootView;
                //if (v != null)
                //{
                //return v;
                //}
            }

            View parent = this;

            while (parent.mParent != null && parent.mParent is View)
            {
                parent = (View)parent.mParent;
            }

            return parent;
        }

        /**
         * Returns the current visibility of the window this view is attached to
         * (either {@link #GONE}, {@link #INVISIBLE}, or {@link #VISIBLE}).
         *
         * @return Returns the current visibility of the view's window.
         */
        public int getWindowVisibility()
        {
            return mAttachInfo != null ? mAttachInfo.mWindowVisibility : GONE;
        }

        /**
         * Set flags controlling behavior of this view.
         *
         * @param flags Constant indicating the value which should be set
         * @param mask Constant indicating the bit range that should be changed
         */
        void setFlags(int flags, int mask)
        {
            int old = mViewFlags;
            mViewFlags = (mViewFlags & ~mask) | (flags & mask);

            int changed = mViewFlags ^ old;
            if (changed == 0)
            {
                return;
            }
            int privateFlags = mPrivateFlags;
            bool shouldNotifyFocusableAvailable = false;

            // If focusable is auto, update the FOCUSABLE bit.
            int focusableChangedByAuto = 0;
            if (((mViewFlags & FOCUSABLE_AUTO) != 0)
                    && (changed & (FOCUSABLE_MASK | CLICKABLE)) != 0)
            {
                // Heuristic only takes into account whether view is clickable.
                int newFocus;
                if ((mViewFlags & CLICKABLE) != 0)
                {
                    newFocus = FOCUSABLE;
                }
                else
                {
                    newFocus = NOT_FOCUSABLE;
                }
                mViewFlags = (mViewFlags & ~FOCUSABLE) | newFocus;
                focusableChangedByAuto = (old & FOCUSABLE) ^ (newFocus & FOCUSABLE);
                changed = (changed & ~FOCUSABLE) | focusableChangedByAuto;
            }

            /* Check if the FOCUSABLE bit has changed */
            if (((changed & FOCUSABLE) != 0) && ((privateFlags & PFLAG_HAS_BOUNDS) != 0))
            {
                if (((old & FOCUSABLE) == FOCUSABLE)
                        && ((privateFlags & PFLAG_FOCUSED) != 0))
                {
                    /* Give up focus if we are no longer focusable */
                    clearFocus();
                    if (mParent is View)
                    {
                        ((View)mParent).clearFocusedInCluster();
                    }
                }
                else if (((old & FOCUSABLE) == NOT_FOCUSABLE)
                      && ((privateFlags & PFLAG_FOCUSED) == 0))
                {
                    /*
                     * Tell the view system that we are now available to take focus
                     * if no one else already has it.
                     */
                    if (mParent != null)
                    {
                        ViewRootImpl viewRootImpl = getViewRootImpl();
                        if (focusableChangedByAuto == 0
                                || viewRootImpl == null
                                || true//viewRootImpl.mThread == Thread.currentThread()
                        )
                        {
                            shouldNotifyFocusableAvailable = canTakeFocus();
                        }
                    }
                }
            }

            int newVisibility = flags & VISIBILITY_MASK;
            if (newVisibility == VISIBLE)
            {
                if ((changed & VISIBILITY_MASK) != 0)
                {
                    /*
                     * If this view is becoming visible, invalidate it in case it changed while
                     * it was not visible. Marking it drawn ensures that the invalidation will
                     * go through.
                     */
                    mPrivateFlags |= PFLAG_DRAWN;
                    invalidate(true);

                    //needGlobalAttributesUpdate(true);

                    // a view becoming visible is worth notifying the parent about in case nothing has
                    // focus. Even if this specific view isn't focusable, it may contain something that
                    // is, so let the root view try to give this focus if nothing else does.
                    shouldNotifyFocusableAvailable = hasSize();
                }
            }

            if ((changed & ENABLED_MASK) != 0)
            {
                if ((mViewFlags & ENABLED_MASK) == ENABLED)
                {
                    // a view becoming enabled should notify the parent as long as the view is also
                    // visible and the parent wasn't already notified by becoming visible during this
                    // setFlags invocation.
                    shouldNotifyFocusableAvailable = canTakeFocus();
                }
                else
                {
                    if (isFocused()) clearFocus();
                }
            }

            if (shouldNotifyFocusableAvailable && mParent != null)
            {
                mParent.focusableViewAvailable(this);
            }

            /* Check if the GONE bit has changed */
            if ((changed & GONE) != 0)
            {
                //needGlobalAttributesUpdate(false);
                requestLayout();

                if (((mViewFlags & VISIBILITY_MASK) == GONE))
                {
                    if (hasFocus())
                    {
                        clearFocus();
                        if (mParent is View)
                        {
                            ((View)mParent).clearFocusedInCluster();
                        }
                    }
                    //clearAccessibilityFocus();
                    //destroyDrawingCache();
                    if (mParent is View)
                    {
                        // GONE views noop invalidation, so invalidate the parent
                        ((View)mParent).invalidate(true);
                    }
                    // Mark the view drawn to ensure that it gets invalidated properly the next
                    // time it is visible and gets invalidated
                    mPrivateFlags |= PFLAG_DRAWN;
                }
                if (mAttachInfo != null)
                {
                    mAttachInfo.mViewVisibilityChanged = true;
                }
            }

            /* Check if the VISIBLE bit has changed */
            if ((changed & INVISIBLE) != 0)
            {
                //needGlobalAttributesUpdate(false);
                /*
                 * If this view is becoming invisible, set the DRAWN flag so that
                 * the next invalidate() will not be skipped.
                 */
                mPrivateFlags |= PFLAG_DRAWN;

                if (((mViewFlags & VISIBILITY_MASK) == INVISIBLE))
                {
                    // root view becoming invisible shouldn't clear focus and accessibility focus
                    if (getRootView() != this)
                    {
                        if (hasFocus())
                        {
                            clearFocus();
                            if (mParent is View)
                            {
                                ((View)mParent).clearFocusedInCluster();
                            }
                        }
                        //clearAccessibilityFocus();
                    }
                }
                if (mAttachInfo != null)
                {
                    mAttachInfo.mViewVisibilityChanged = true;
                }
            }

            if ((changed & VISIBILITY_MASK) != 0)
            {
                // If the view is invisible, cleanup its display list to free up resources
                if (newVisibility != VISIBLE && mAttachInfo != null)
                {
                    cleanupDraw();
                }

                if (mParent is View)
                {
                    View parent = (View)mParent;
                    //parent.onChildVisibilityChanged(this, (changed & VISIBILITY_MASK),
                    //        newVisibility);
                    parent.invalidate(true);
                }
                else if (mParent != null)
                {
                    //Parent.invalidateChild(this, null);
                }

                if (mAttachInfo != null)
                {
                    dispatchVisibilityChanged(this, newVisibility);

                    // Aggregated visibility changes are dispatched to attached views
                    // in visible windows where the parent is currently shown/drawn
                    // or the parent is not a View (and therefore assumed to be a ViewRoot),
                    // discounting clipping or overlapping. This makes it a good place
                    // to change animation states.
                    if (mParent != null && getWindowVisibility() == VISIBLE &&
                            ((!(mParent is View)) || ((View)mParent).isShown()))
                    {
                        dispatchVisibilityAggregated(newVisibility == VISIBLE);
                    }
                    // If this view is invisible from visible, then sending the A11y event by its
                    // parent which is shown and has the accessibility important.
                    if ((old & VISIBILITY_MASK) == VISIBLE)
                    {
                        //notifySubtreeAccessibilityStateChangedByParentIfNeeded();
                    }
                    else
                    {
                        //notifySubtreeAccessibilityStateChangedIfNeeded();
                    }
                }
            }

            //if ((changed & WILL_NOT_CACHE_DRAWING) != 0)
            //{
            //    destroyDrawingCache();
            //}

            //if ((changed & DRAWING_CACHE_ENABLED) != 0)
            //{
            //    destroyDrawingCache();
            //    mPrivateFlags &= ~PFLAG_DRAWING_CACHE_VALID;
            //    invalidateParentCaches();
            //}

            //if ((changed & DRAWING_CACHE_QUALITY_MASK) != 0)
            //{
            //    destroyDrawingCache();
            //    mPrivateFlags &= ~PFLAG_DRAWING_CACHE_VALID;
            //}

            if ((changed & DRAW_MASK) != 0)
            {
                if ((mViewFlags & WILL_NOT_DRAW) != 0)
                {
                    //if (mBackground != null
                    //|| mDefaultFocusHighlight != null
                    //|| (mForegroundInfo != null && mForegroundInfo.mDrawable != null))
                    //{
                    //mPrivateFlags &= ~PFLAG_SKIP_DRAW;
                    //}
                    //else
                    //{
                    mPrivateFlags |= PFLAG_SKIP_DRAW;
                    //}
                }
                else
                {
                    mPrivateFlags &= ~PFLAG_SKIP_DRAW;
                }
                requestLayout();
                invalidate(true);
            }

            //if ((changed & KEEP_SCREEN_ON) != 0)
            //{
            //    if (mParent != null && mAttachInfo != null && !mAttachInfo.mRecomputeGlobalAttributes)
            //    {
            //        mParent.recomputeViewAttributes(this);
            //    }
            //}

            //if (accessibilityEnabled)
            //{
            //    // If we're an accessibility pane and the visibility changed, we already have sent
            //    // a state change, so we really don't need to report other changes.
            //    if (isAccessibilityPane())
            //    {
            //        changed &= ~VISIBILITY_MASK;
            //    }
            //    if ((changed & FOCUSABLE) != 0 || (changed & VISIBILITY_MASK) != 0
            //            || (changed & CLICKABLE) != 0 || (changed & LONG_CLICKABLE) != 0
            //            || (changed & CONTEXT_CLICKABLE) != 0)
            //    {
            //        if (oldIncludeForAccessibility != includeForAccessibility())
            //        {
            //            notifySubtreeAccessibilityStateChangedIfNeeded();
            //        }
            //        else
            //        {
            //            notifyViewAccessibilityStateChangedIfNeeded(
            //                    AccessibilityEvent.CONTENT_CHANGE_TYPE_UNDEFINED);
            //        }
            //    }
            //    else if ((changed & ENABLED_MASK) != 0)
            //    {
            //        notifyViewAccessibilityStateChangedIfNeeded(
            //                AccessibilityEvent.CONTENT_CHANGE_TYPE_UNDEFINED);
            //    }
            //}
        }

        /**
         * This view won't draw. {@link #onDraw(android.graphics.Canvas)} won't be
         * called and further optimizations will be performed. It is okay to have
         * this flag set and a background. Use with DRAW_MASK when calling setFlags.
         * {@hide}
         */
        internal const int WILL_NOT_DRAW = 0x00000080;

        /**
         * Mask for use with setFlags indicating bits used for indicating whether
         * this view is will draw
         * {@hide}
         */
        internal const int DRAW_MASK = 0x00000080;

        /**
         * <p>This view doesn't show scrollbars.</p>
         * {@hide}
         */
        const int SCROLLBARS_NONE = 0x00000000;

        /**
         * <p>This view shows horizontal scrollbars.</p>
         * {@hide}
         */
        const int SCROLLBARS_HORIZONTAL = 0x00000100;

        /**
         * <p>This view shows vertical scrollbars.</p>
         * {@hide}
         */
        const int SCROLLBARS_VERTICAL = 0x00000200;

        /**
         * <p>Mask for use with setFlags indicating bits used for indicating which
         * scrollbars are enabled.</p>
         * {@hide}
         */
        const int SCROLLBARS_MASK = 0x00000300;

        /**
         * Returns true if this view is focusable or if it contains a reachable View
         * for which {@link #hasFocusable()} returns {@code true}. A "reachable hasFocusable()"
         * is a view whose parents do not block descendants focus.
         * Only {@link #VISIBLE} views are considered focusable.
         *
         * <p>As of {@link Build.VERSION_CODES#O} views that are determined to be focusable
         * through {@link #FOCUSABLE_AUTO} will also cause this method to return {@code true}.
         * Apps that declare a {@link android.content.pm.ApplicationInfo#targetSdkVersion} of
         * earlier than {@link Build.VERSION_CODES#O} will continue to see this method return
         * {@code false} for views not explicitly marked as focusable.
         * Use {@link #hasExplicitFocusable()} if you require the pre-{@link Build.VERSION_CODES#O}
         * behavior.</p>
         *
         * @return {@code true} if the view is focusable or if the view contains a focusable
         *         view, {@code false} otherwise
         *
         * @see View#FOCUS_BLOCK_DESCENDANTS
         * @see View#getTouchscreenBlocksFocus()
         * @see #hasExplicitFocusable()
         */
        public bool hasFocusable()
        {
            return hasFocusable(!false, false);
        }

        bool hasFocusable(bool allowAutoFocus, bool dispatchExplicit)
        {
            if (!isFocusableInTouchMode())
            {
                for (Parent p = mParent; p is View; p = p.getParent())
                {
                    View g = (View)p;
                    if (g.shouldBlockFocusForTouchscreen())
                    {
                        return false;
                    }
                }
            }

            // Invisible, gone, or disabled views are never focusable.
            if ((mViewFlags & VISIBILITY_MASK) != VISIBLE
                    || (mViewFlags & ENABLED_MASK) != ENABLED)
            {
                return false;
            }

            // Only use effective focusable value when allowed.
            if ((allowAutoFocus || getFocusable() != FOCUSABLE_AUTO) && isFocusable())
            {
                return true;
            }

            return false;
        }

        /**
         * Returns whether this View is currently able to take focus.
         *
         * @return True if this view can take focus, or false otherwise.
         */
        public bool isFocusable()
        {
            return FOCUSABLE == (mViewFlags & FOCUSABLE);
        }

        /**
         * Returns the focusable setting for this view.
         *
         * @return One of {@link #NOT_FOCUSABLE}, {@link #FOCUSABLE}, or {@link #FOCUSABLE_AUTO}.
         * @attr ref android.R.styleable#View_focusable
         */
        public int getFocusable()
        {
            return (mViewFlags & FOCUSABLE_AUTO) > 0 ? FOCUSABLE_AUTO : mViewFlags & FOCUSABLE;
        }

        /**
         * The view's identifier.
         * {@hide}
         *
         * @see #setId(int)
         * @see #getId()
         */
        int mID = NO_ID;

        class MatchIdPredicate
        {
            public int mId;

            public static implicit operator Predicate<View>(MatchIdPredicate match) => t => t.mID == match.mId;

        }

        /**
         * Predicate for matching a view by its id.
         */
        private MatchIdPredicate mMatchIdPredicate;

        private View findViewInsideOutShouldExist(View root, int id)
        {
            return findViewInsideOutShouldExist(root, this, id);
        }

        private View findViewInsideOutShouldExist(View root, View start, int id)
        {
            if (mMatchIdPredicate == null)
            {
                mMatchIdPredicate = new MatchIdPredicate();
            }
            mMatchIdPredicate.mId = id;
            View result = root.findViewByPredicateInsideOut<View>(start, mMatchIdPredicate);
            if (result == null)
            {
                Console.WriteLine("couldn't find view with id " + id);
            }
            return result;
        }

        /**
         * @param predicate The predicate to evaluate.
         * @param childToSkip If not null, ignores this child during the recursive traversal.
         * @return The first view that matches the predicate or null.
         * @hide
         */
        protected T findViewByPredicateTraversal<T>(Predicate<View> predicate,
                View childToSkip) where T : View
        {
            if (predicate(this))
            {
                return (T)this;
            }
            return null;
        }

        /**
         * Look for a child view that matches the specified predicate.
         * If this view matches the predicate, return this view.
         *
         * @param predicate The predicate to evaluate.
         * @return The first view that matches the predicate or null.
         * @hide
         */
        public T findViewByPredicate<T>(Predicate<View> predicate) where T : View
        {
            return findViewByPredicateTraversal<T>(predicate, null);
        }

        /**
         * Look for a child view that matches the specified predicate,
         * starting with the specified view and its descendents and then
         * recusively searching the ancestors and siblings of that view
         * until this view is reached.
         *
         * This method is useful in cases where the predicate does not match
         * a single unique view (perhaps multiple views use the same id)
         * and we are trying to find the view that is "closest" in scope to the
         * starting view.
         *
         * @param start The view to start from.
         * @param predicate The predicate to evaluate.
         * @return The first view that matches the predicate or null.
         * @hide
         */
        public T findViewByPredicateInsideOut<T>(
               View start, Predicate<View> predicate) where T : View
        {
            View childToSkip = null;
            for (; ; )
            {
                T view = start.findViewByPredicateTraversal<T>(predicate, childToSkip);
                if (view != null || start == this)
                {
                    return view;
                }

                Parent parent = start.mParent;
                if (parent == null || !(parent is View))
                {
                    return null;
                }

                childToSkip = start;
                start = (View)parent;
            }
        }

        /**
         * Gives focus to the default-focus view in the view hierarchy that has this view as a root.
         * If the default-focus view cannot be found, falls back to calling {@link #requestFocus(int)}.
         *
         * @return Whether this view or one of its descendants actually took focus
         */
        public bool restoreDefaultFocus()
        {
            return requestFocus(View.FOCUS_DOWN);
        }

        /**
         * Call this to try to give focus to a specific view or to one of its
         * descendants.
         *
         * A view will not actually take focus if it is not focusable ({@link #isFocusable} returns
         * false), or if it can't be focused due to other conditions (not focusable in touch mode
         * ({@link #isFocusableInTouchMode}) while the device is in touch mode, not visible, not
         * enabled, or has no size).
         *
         * See also {@link #focusSearch(int)}, which is what you call to say that you
         * have focus, and you want your parent to look for the next one.
         *
         * This is equivalent to calling {@link #requestFocus(int, Rect)} with arguments
         * {@link #FOCUS_DOWN} and <code>null</code>.
         *
         * @return Whether this view or one of its descendants actually took focus.
         */
        public bool requestFocus()
        {
            return requestFocus(View.FOCUS_DOWN);
        }

        /**
         * This will request focus for whichever View was last focused within this
         * cluster before a focus-jump out of it.
         *
         * @hide
         */
        public bool restoreFocusInCluster(int direction)
        {
            // Prioritize focusableByDefault over algorithmic focus selection.
            if (restoreDefaultFocus())
            {
                return true;
            }
            return requestFocus(direction);
        }

        /**
         * This will request focus for whichever View not in a cluster was last focused before a
         * focus-jump to a cluster. If no non-cluster View has previously had focus, this will focus
         * the "first" focusable view it finds.
         *
         * @hide
         */
        public bool restoreFocusNotInCluster()
        {
            return requestFocus(View.FOCUS_DOWN);
        }

        /**
         * Call this to try to give focus to a specific view or to one of its
         * descendants and give it a hint about what direction focus is heading.
         *
         * A view will not actually take focus if it is not focusable ({@link #isFocusable} returns
         * false), or if it is focusable and it is not focusable in touch mode
         * ({@link #isFocusableInTouchMode}) while the device is in touch mode.
         *
         * See also {@link #focusSearch(int)}, which is what you call to say that you
         * have focus, and you want your parent to look for the next one.
         *
         * This is equivalent to calling {@link #requestFocus(int, Rect)} with
         * <code>null</code> set for the previously focused rectangle.
         *
         * @param direction One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, and FOCUS_RIGHT
         * @return Whether this view or one of its descendants actually took focus.
         */
        public bool requestFocus(int direction)
        {
            return requestFocus(direction, null);
        }

        /**
         * Call this to try to give focus to a specific view or to one of its descendants
         * and give it hints about the direction and a specific rectangle that the focus
         * is coming from.  The rectangle can help give larger views a finer grained hint
         * about where focus is coming from, and therefore, where to show selection, or
         * forward focus change internally.
         *
         * A view will not actually take focus if it is not focusable ({@link #isFocusable} returns
         * false), or if it is focusable and it is not focusable in touch mode
         * ({@link #isFocusableInTouchMode}) while the device is in touch mode.
         *
         * A View will not take focus if it is not visible.
         *
         * A View will not take focus if one of its parents has
         * {@link android.view.View#getDescendantFocusability()} equal to
         * {@link View#FOCUS_BLOCK_DESCENDANTS}.
         *
         * See also {@link #focusSearch(int)}, which is what you call to say that you
         * have focus, and you want your parent to look for the next one.
         *
         * You may wish to override this method if your custom {@link View} has an internal
         * {@link View} that it wishes to forward the request to.
         *
         * @param direction One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, and FOCUS_RIGHT
         * @param previouslyFocusedRect The rectangle (in this View's coordinate system)
         *        to give a finer grained hint about where focus is coming from.  May be null
         *        if there is no hint.
         * @return Whether this view or one of its descendants actually took focus.
         */
        public bool requestFocus(int direction, Rect previouslyFocusedRect)
        {
            return requestFocusNoSearch(direction, previouslyFocusedRect);
        }

        /**
         * Returns whether the device is currently in touch mode.  Touch mode is entered
         * once the user begins interacting with the device by touch, and affects various
         * things like whether focus is always visible to the user.
         *
         * @return Whether the device is in touch mode.
         */
        public bool isInTouchMode()
        {
            if (mAttachInfo != null)
            {
                return mAttachInfo.mInTouchMode;
            }
            else
            {
                return ViewRootImpl.isInTouchMode();
            }
        }

        private bool requestFocusNoSearch(int direction, Rect previouslyFocusedRect)
        {
            // need to be focusable
            if (!canTakeFocus())
            {
                return false;
            }

            // need to be focusable in touch mode if in touch mode
            if (isInTouchMode() &&
                (FOCUSABLE_IN_TOUCH_MODE != (mViewFlags & FOCUSABLE_IN_TOUCH_MODE)))
            {
                return false;
            }

            // need to not have any parents blocking us
            if (hasAncestorThatBlocksDescendantFocus())
            {
                return false;
            }

            if (!isLayoutValid())
            {
                mPrivateFlags |= PFLAG_WANTS_FOCUS;
            }
            else
            {
                clearParentsWantFocus();
            }

            handleFocusGainInternal(direction, previouslyFocusedRect);
            return true;
        }

        public void requestChildFocus(View child, View focused)
        {
            if (getDescendantFocusability() == FOCUS_BLOCK_DESCENDANTS)
            {
                return;
            }

            // Unfocus us, if necessary
            clearFocusInternal(focused, false, false);

            // We had a previous notion of who had focus. Clear it.
            if (mFocused != child)
            {
                if (mFocused != null)
                {
                    mFocused.unFocus(focused);
                }

                mFocused = child;
            }
            if (mParent != null)
            {
                mParent.requestChildFocus(this, focused);
            }
        }

        /**
         * Give this view focus. This will cause
         * {@link #onFocusChanged(bool, int, android.graphics.Rect)} to be called.
         *
         * Note: this does not check whether this {@link View} should get focus, it just
         * gives it focus no matter what.  It should only be called internally by framework
         * code that knows what it is doing, namely {@link #requestFocus(int, Rect)}.
         *
         * @param direction values are {@link View#FOCUS_UP}, {@link View#FOCUS_DOWN},
         *        {@link View#FOCUS_LEFT} or {@link View#FOCUS_RIGHT}. This is the direction which
         *        focus moved when requestFocus() is called. It may not always
         *        apply, in which case use the default View.FOCUS_DOWN.
         * @param previouslyFocusedRect The rectangle of the view that had focus
         *        prior in this View's coordinate system.
         */
        void handleFocusGainInternal(int direction, Rect previouslyFocusedRect)
        {
            if (DBG)
            {
                System.Console.WriteLine(this + " requestFocus()");
            }

            if ((mPrivateFlags & PFLAG_FOCUSED) == 0)
            {
                mPrivateFlags |= PFLAG_FOCUSED;

                View oldFocus = (mAttachInfo != null) ? null : null; //getRootView().findFocus() : null;

                if (mParent != null)
                {
                    mParent.requestChildFocus(this, this);
                    updateFocusedInCluster(oldFocus, direction);
                }

                if (mAttachInfo != null)
                {
                    //mAttachInfo.mTreeObserver.dispatchOnGlobalFocusChange(oldFocus, this);
                }

                onFocusChanged(true, direction, previouslyFocusedRect);
                //refreshDrawableState();
            }
        }

        /**
         * When set, focus will not be permitted to enter this group if a touchscreen is present.
         */
        internal const int FLAG_TOUCHSCREEN_BLOCKS_FOCUS = 0x4000000;

        internal const int FLAG_MASK_FOCUSABILITY = 0x60000;

        /**
         * This view will get focus before any of its descendants.
         */
        public const int FOCUS_BEFORE_DESCENDANTS = 0x20000;

        /**
         * This view will get focus only if none of its descendants want it.
         */
        public const int FOCUS_AFTER_DESCENDANTS = 0x40000;

        /**
         * This view will block any of its descendants from getting focus, even
         * if they are focusable.
         */
        public const int FOCUS_BLOCK_DESCENDANTS = 0x60000;

        /**
         * Used to map between enum in attrubutes and flag values.
         */
        private readonly int[] DESCENDANT_FOCUSABILITY_FLAGS =
                {FOCUS_BEFORE_DESCENDANTS, FOCUS_AFTER_DESCENDANTS,
                    FOCUS_BLOCK_DESCENDANTS};

        public const int NO_ID = -1;

        /**
         * Gets the descendant focusability of this view group.  The descendant
         * focusability defines the relationship between this view group and its
         * descendants when looking for a view to take focus in
         * {@link #requestFocus(int, android.graphics.Rect)}.
         *
         * @return one of {@link #FOCUS_BEFORE_DESCENDANTS}, {@link #FOCUS_AFTER_DESCENDANTS},
         *   {@link #FOCUS_BLOCK_DESCENDANTS}.
         */
        public int getDescendantFocusability()
        {
            return mGroupFlags & FLAG_MASK_FOCUSABILITY;
        }

        /**
         * Find the nearest view in the specified direction that wants to take
         * focus.
         *
         * @param focused The view that currently has focus
         * @param direction One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, and
         *        FOCUS_RIGHT, or 0 for not applicable.
         */
        public View focusSearch(View focused, int direction)
        {
            if (isRootNamespace())
            {
                // root namespace means we should consider ourselves the top of the
                // tree for focus searching; otherwise we could be focus searching
                // into other tabs.  see LocalActivityManager and TabHost for more info.
                return FocusFinder.getInstance().findNextFocus(this, focused, direction);
            }
            else if (mParent != null)
            {
                return mParent.focusSearch(focused, direction);
            }
            return null;
        }

        /**
         * Find the nearest view in the specified direction that can take focus.
         * This does not actually give focus to that view.
         *
         * @param direction One of FOCUS_UP, FOCUS_DOWN, FOCUS_LEFT, and FOCUS_RIGHT
         *
         * @return The nearest focusable in the specified direction, or null if none
         *         can be found.
         */
        public View focusSearch(int direction)
        {
            if (mParent != null)
            {
                return mParent.focusSearch(this, direction);
            }
            else
            {
                return null;
            }
        }

        /**
         * The offset, in pixels, by which the content of this view is scrolled
         * horizontally.
         * Please use {@link View#getScrollX()} and {@link View#setScrollX(int)} instead of
         * accessing these directly.
         * {@hide}
         */
        protected int mScrollX;

        /**
        * The offset, in pixels, by which the content of this view is scrolled
        * vertically.
        * Please use {@link View#getScrollY()} and {@link View#setScrollY(int)} instead of
        * accessing these directly.
        * {@hide}
        */
        protected int mScrollY;

        /**
         * Return the scrolled left position of this view. This is the left edge of
         * the displayed part of your view. You do not need to draw any pixels
         * farther left, since those are outside of the frame of your view on
         * screen.
         *
         * @return The left edge of the displayed part of your view, in pixels.
         */
        public int getScrollX()
        {
            return mScrollX;
        }

        /**
         * Return the scrolled top position of this view. This is the top edge of
         * the displayed part of your view. You do not need to draw any pixels above
         * it, since those are outside of the frame of your view on screen.
         *
         * @return The top edge of the displayed part of your view, in pixels.
         */
        public int getScrollY()
        {
            return mScrollY;
        }

        /**
         * Return the visible drawing bounds of your view. Fills in the output
         * rectangle with the values from getScrollX(), getScrollY(),
         * getWidth(), and getHeight(). These bounds do not account for any
         * transformation properties currently set on the view, such as
         * {@link #setScaleX(float)} or {@link #setRotation(float)}.
         *
         * @param outRect The (scrolled) drawing bounds of the view.
         */
        public void getDrawingRect(Rect outRect)
        {
            outRect.left = mScrollX;
            outRect.top = mScrollY;
            outRect.right = mScrollX + (mRight - mLeft);
            outRect.bottom = mScrollY + (mBottom - mTop);
        }

        /**
         * When a view has focus and the user navigates away from it, the next view is searched for
         * starting from the rectangle filled in by this method.
         *
         * By default, the rectangle is the {@link #getDrawingRect(android.graphics.Rect)})
         * of the view.  However, if your view maintains some idea of internal selection,
         * such as a cursor, or a selected row or column, you should override this method and
         * fill in a more specific rectangle.
         *
         * @param r The rectangle to fill in, in this view's coordinates.
         */
        public void getFocusedRect(Rect r)
        {
            getDrawingRect(r);
        }

        /**
         * {@hide}
         *
         * @param isRoot true if the view belongs to the root namespace, false
         *        otherwise
         */
        public void setIsRootNamespace(bool isRoot)
        {
            if (isRoot)
            {
                mPrivateFlags |= PFLAG_IS_ROOT_NAMESPACE;
            }
            else
            {
                mPrivateFlags &= ~PFLAG_IS_ROOT_NAMESPACE;
            }
        }

        /**
         * {@hide}
         *
         * @return true if the view belongs to the root namespace, false otherwise
         */
        public bool isRootNamespace()
        {
            return (mPrivateFlags & PFLAG_IS_ROOT_NAMESPACE) != 0;
        }

        /**
         * When this view has focus and the next focus is {@link #FOCUS_LEFT},
         * the user may specify which view to go to next.
         */
        private int mNextFocusLeftId = View.NO_ID;

        /**
         * When this view has focus and the next focus is {@link #FOCUS_RIGHT},
         * the user may specify which view to go to next.
         */
        private int mNextFocusRightId = View.NO_ID;

        /**
         * When this view has focus and the next focus is {@link #FOCUS_UP},
         * the user may specify which view to go to next.
         */
        private int mNextFocusUpId = View.NO_ID;

        /**
         * When this view has focus and the next focus is {@link #FOCUS_DOWN},
         * the user may specify which view to go to next.
         */
        private int mNextFocusDownId = View.NO_ID;

        /**
         * When this view has focus and the next focus is {@link #FOCUS_FORWARD},
         * the user may specify which view to go to next.
         */
        int mNextFocusForwardId = View.NO_ID;

        /**
         * User-specified next keyboard navigation cluster in the {@link #FOCUS_FORWARD} direction.
         *
         * @see #findUserSetNextKeyboardNavigationCluster(View, int)
         */
        int mNextClusterForwardId = View.NO_ID;

        /**
         * Returns whether this View is a root of a keyboard navigation cluster.
         *
         * @return True if this view is a root of a cluster, or false otherwise.
         * @attr ref android.R.styleable#View_keyboardNavigationCluster
         */
        public bool isKeyboardNavigationCluster()
        {
            return (mPrivateFlags & PFLAG_CLUSTER) != 0;
        }

        /**
         * Searches up the view hierarchy to find the top-most cluster. All deeper/nested clusters
         * will be ignored.
         *
         * @return the keyboard navigation cluster that this view is in (can be this view)
         *         or {@code null} if not in one
         */
        View findKeyboardNavigationCluster()
        {
            if (mParent is View)
            {
                View cluster = ((View)mParent).findKeyboardNavigationCluster();
                if (cluster != null)
                {
                    return cluster;
                }
                else if (isKeyboardNavigationCluster())
                {
                    return this;
                }
            }
            return null;
        }

        /**
         * Set whether this view is a root of a keyboard navigation cluster.
         *
         * @param isCluster If true, this view is a root of a cluster.
         *
         * @attr ref android.R.styleable#View_keyboardNavigationCluster
         */
        public void setKeyboardNavigationCluster(bool isCluster)
        {
            if (isCluster)
            {
                mPrivateFlags |= PFLAG_CLUSTER;
            }
            else
            {
                mPrivateFlags &= ~PFLAG_CLUSTER;
            }
        }

        /**
         * Sets this View as the one which receives focus the next time cluster navigation jumps
         * to the cluster containing this View. This does NOT change focus even if the cluster
         * containing this view is current.
         *
         * @hide
         */
        public void setFocusedInCluster()
        {
            setFocusedInCluster(findKeyboardNavigationCluster());
        }

        private void setFocusedInCluster(View cluster)
        {
            if (this is View)
            {
                ((View)this).mFocusedInCluster = null;
            }
            if (cluster == this)
            {
                return;
            }
            Parent parent = mParent;
            View child = this;
            while (parent is View)
            {
                ((View)parent).mFocusedInCluster = child;
                if (parent == cluster)
                {
                    break;
                }
                child = (View)parent;
                parent = parent.getParent();
            }
        }

        private void updateFocusedInCluster(View oldFocus, int direction)
        {
            if (oldFocus != null)
            {
                View oldCluster = oldFocus.findKeyboardNavigationCluster();
                View cluster = findKeyboardNavigationCluster();
                if (oldCluster != cluster)
                {
                    // Going from one cluster to another, so save last-focused.
                    // This covers cluster jumps because they are always FOCUS_DOWN
                    oldFocus.setFocusedInCluster(oldCluster);
                    if (!(oldFocus.mParent is View))
                    {
                        return;
                    }
                    if (direction == FOCUS_FORWARD || direction == FOCUS_BACKWARD)
                    {
                        // This is a result of ordered navigation so consider navigation through
                        // the previous cluster "complete" and clear its last-focused memory.
                        ((View)oldFocus.mParent).clearFocusedInCluster(oldFocus);
                    }
                    else if (oldFocus is View
                        && ((View)oldFocus).getDescendantFocusability()
                                == View.FOCUS_AFTER_DESCENDANTS
                        && ViewRootImpl.isViewDescendantOf(this, oldFocus))
                    {
                        // This means oldFocus is not focusable since it obviously has a focusable
                        // child (this). Don't restore focus to it in the future.
                        ((View)oldFocus.mParent).clearFocusedInCluster(oldFocus);
                    }
                }
            }
        }

        /**
         * Removes {@code child} (and associated focusedInCluster chain) from the cluster containing
         * it.
         * <br>
         * This is intended to be run on {@code child}'s immediate parent. This is necessary because
         * the chain is sometimes cleared after {@code child} has been detached.
         */
        void clearFocusedInCluster(View child)
        {
            if (mFocusedInCluster != child)
            {
                return;
            }
            clearFocusedInCluster();
        }

        /**
         * Removes the focusedInCluster chain from this up to the cluster containing it.
         */
        void clearFocusedInCluster()
        {
            View top = findKeyboardNavigationCluster();
            Parent parent = this;
            do
            {
                ((View)parent).mFocusedInCluster = null;
                if (parent == top)
                {
                    break;
                }
                parent = parent.getParent();
            } while (parent is View);
        }

        /**
         * Returns the focused child of this view, if any. The child may have focus
         * or contain focus.
         *
         * @return the focused child or null.
         */
        public View getFocusedChild()
        {
            return mFocused;
        }

        View getDeepestFocusedChild()
        {
            View v = this;
            while (v != null)
            {
                if (v.isFocused())
                {
                    return v;
                }
                v = v is View ? v.getFocusedChild() : null;
            }
            return null;
        }

        /**
         * Set whether this View should ignore focus requests for itself and its children.
         * If this option is enabled and the View or a descendant currently has focus, focus
         * will proceed forward.
         *
         * @param touchscreenBlocksFocus true to enable blocking focus in the presence of a touchscreen
         */
        public void setTouchscreenBlocksFocus(bool touchscreenBlocksFocus)
        {
            if (touchscreenBlocksFocus)
            {
                mGroupFlags |= FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
                if (hasFocus() && !isKeyboardNavigationCluster())
                {
                    View focusedChild = getDeepestFocusedChild();
                    if (!focusedChild.isFocusableInTouchMode())
                    {
                        View newFocus = focusSearch(FOCUS_FORWARD);
                        if (newFocus != null)
                        {
                            newFocus.requestFocus();
                        }
                    }
                }
            }
            else
            {
                mGroupFlags &= ~FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
            }
        }

        private void setTouchscreenBlocksFocusNoRefocus(bool touchscreenBlocksFocus)
        {
            if (touchscreenBlocksFocus)
            {
                mGroupFlags |= FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
            }
            else
            {
                mGroupFlags &= ~FLAG_TOUCHSCREEN_BLOCKS_FOCUS;
            }
        }

        /**
         * Check whether this View should ignore focus requests for itself and its children.
         */
        public bool getTouchscreenBlocksFocus()
        {
            return (mGroupFlags & FLAG_TOUCHSCREEN_BLOCKS_FOCUS) != 0;
        }

        bool shouldBlockFocusForTouchscreen()
        {
            // There is a special case for keyboard-navigation clusters. We allow cluster navigation
            // to jump into blockFocusForTouchscreen Views which are clusters. Once in the
            // cluster, focus is free to move around within it.
            bool FEATURE_TOUCHSCREEN = true;//mContext.getPackageManager().hasSystemFeature(PackageManager.FEATURE_TOUCHSCREEN);
            return getTouchscreenBlocksFocus() &&
                    FEATURE_TOUCHSCREEN
                    && !(isKeyboardNavigationCluster()
                            && (hasFocus() || (findKeyboardNavigationCluster() != this)));
        }

        /**
         * @return Whether any ancestor of this view blocks descendant focus.
         */
        private bool hasAncestorThatBlocksDescendantFocus()
        {
            bool focusableInTouchMode = true;
            Parent ancestor = mParent;
            while (ancestor is View)
            {
                View vgAncestor = (View)ancestor;
                if (vgAncestor.getDescendantFocusability() == FOCUS_BLOCK_DESCENDANTS
                        || (!focusableInTouchMode && vgAncestor.shouldBlockFocusForTouchscreen()))
                {
                    return true;
                }
                else
                {
                    ancestor = vgAncestor.mParent;
                }
            }
            return false;
        }

        void clearParentsWantFocus()
        {
            if (mParent is View)
            {
                ((View)mParent).mPrivateFlags &= ~PFLAG_WANTS_FOCUS;
                ((View)mParent).clearParentsWantFocus();
            }
        }

        /**
         * Clears focus from the view, optionally propagating the change up through
         * the parent hierarchy and requesting that the root view place new focus.
         *
         * @param propagate whether to propagate the change up through the parent
         *            hierarchy
         * @param refocus when propagate is true, specifies whether to request the
         *            root view place new focus
         */
        internal void clearFocusInternal(View focused, bool propagate, bool refocus)
        {
            if ((mPrivateFlags & PFLAG_FOCUSED) != 0)
            {
                mPrivateFlags &= ~PFLAG_FOCUSED;
                clearParentsWantFocus();

                if (propagate && mParent != null)
                {
                    mParent.clearChildFocus(this);
                }

                onFocusChanged(false, 0, null);
                //refreshDrawableState();

                if (propagate && (!refocus || !rootViewRequestFocus()))
                {
                    //notifyGlobalFocusCleared(this);
                }
            }
        }

        public void layout(int l, int t, int r, int b)
        {
            if ((mPrivateFlags & PFLAG_MEASURE_NEEDED_BEFORE_LAYOUT) != 0)
            {
                onMeasure(mOldWidthMeasureSpec, mOldHeightMeasureSpec);
                mPrivateFlags &= ~PFLAG_MEASURE_NEEDED_BEFORE_LAYOUT;
            }

            int oldL = mLeft;
            int oldT = mTop;
            int oldB = mBottom;
            int oldR = mRight;

            bool changed = setFrame(l, t, r, b);

            if (changed || (mPrivateFlags & PFLAG_LAYOUT_REQUIRED) == PFLAG_LAYOUT_REQUIRED)
            {
                onLayout(changed, l, t, r, b);

                //if (shouldDrawRoundScrollbar())
                //{
                //    if (mRoundScrollbarRenderer == null)
                //    {
                //        mRoundScrollbarRenderer = new RoundScrollbarRenderer(this);
                //    }
                //}
                //else
                //{
                //    mRoundScrollbarRenderer = null;
                //}

                mPrivateFlags &= ~PFLAG_LAYOUT_REQUIRED;

                //ListenerInfo li = mListenerInfo;
                //if (li != null && li.mOnLayoutChangeListeners != null)
                //{
                //    ArrayList<OnLayoutChangeListener> listenersCopy =
                //            (ArrayList<OnLayoutChangeListener>)li.mOnLayoutChangeListeners.clone();
                //    int numListeners = listenersCopy.size();
                //    for (int i = 0; i < numListeners; ++i)
                //    {
                //        listenersCopy.get(i).onLayoutChange(this, l, t, r, b, oldL, oldT, oldR, oldB);
                //    }
                //}
            }

            bool wasLayoutValid = isLayoutValid();

            mPrivateFlags &= ~PFLAG_FORCE_LAYOUT;
            mPrivateFlags |= PFLAG_IS_LAID_OUT;

            if (!wasLayoutValid && isFocused())
            {
                mPrivateFlags &= ~PFLAG_WANTS_FOCUS;
                if (canTakeFocus())
                {
                    // We have a robust focus, so parents should no longer be wanting focus.
                    clearParentsWantFocus();
                }
                else if (getViewRootImpl() == null || !getViewRootImpl().isInLayout())
                {
                    // This is a weird case. Most-likely the user, rather than ViewRootImpl, called
                    // layout. In this case, there's no guarantee that parent layouts will be evaluated
                    // and thus the safest action is to clear focus here.
                    clearFocusInternal(null, /* propagate */ true, /* refocus */ false);
                    clearParentsWantFocus();
                }
                else if (!hasParentWantsFocus())
                {
                    // original requestFocus was likely on this view directly, so just clear focus
                    clearFocusInternal(null, /* propagate */ true, /* refocus */ false);
                }
                // otherwise, we let parents handle re-assigning focus during their layout passes.
            }
            else if ((mPrivateFlags & PFLAG_WANTS_FOCUS) != 0)
            {
                mPrivateFlags &= ~PFLAG_WANTS_FOCUS;
                View focused = findFocus();
                if (focused != null)
                {
                    // Try to restore focus as close as possible to our starting focus.
                    if (!restoreDefaultFocus() && !hasParentWantsFocus())
                    {
                        // Give up and clear focus once we've reached the top-most parent which wants
                        // focus.
                        focused.clearFocusInternal(null, /* propagate */ true, /* refocus */ false);
                    }
                }
            }

            //if ((mPrivateFlags3 & PFLAG3_NOTIFY_AUTOFILL_ENTER_ON_LAYOUT) != 0)
            //{
            //    mPrivateFlags3 &= ~PFLAG3_NOTIFY_AUTOFILL_ENTER_ON_LAYOUT;
            //    notifyEnterOrExitForAutoFillIfNeeded(true);
            //}

            //notifyAppearedOrDisappearedForContentCaptureIfNeeded(true);
        }

        /**
         * This is called during layout when the size of this view has changed. If
         * you were just added to the view hierarchy, you're called with the old
         * values of 0.
         *
         * @param w Current width of this view.
         * @param h Current height of this view.
         * @param oldw Old width of this view.
         * @param oldh Old height of this view.
         */
        protected void onSizeChanged(int w, int h, int oldw, int oldh)
        {
        }

        private const bool sCanFocusZeroSized = false;

        // Whether layout calls are currently being suppressed, controlled by calls to
        // suppressLayout()
        bool mSuppressLayout = false;

        /**
         * Returns whether layout calls on this container are currently being
         * suppressed, due to an earlier call to {@link #suppressLayout(bool)}.
         *
         * @return true if layout calls are currently suppressed, false otherwise.
         */
        public bool isLayoutSuppressed()
        {
            return mSuppressLayout;
        }

        private void sizeChange(int newWidth, int newHeight, int oldWidth, int oldHeight)
        {
            onSizeChanged(newWidth, newHeight, oldWidth, oldHeight);
            //if (mOverlay != null)
            //{
            //    mOverlay.getOverlayView().setRight(newWidth);
            //    mOverlay.getOverlayView().setBottom(newHeight);
            //}
            // If this isn't laid out yet, focus assignment will be handled during the "deferment/
            // backtracking" of requestFocus during layout, so don't touch focus here.
            if (!sCanFocusZeroSized && isLayoutValid()
                    // Don't touch focus if animating
                    && !(mParent is View && ((View)mParent).isLayoutSuppressed()))
            {
                if (newWidth <= 0 || newHeight <= 0)
                {
                    if (hasFocus())
                    {
                        clearFocus();
                        if (mParent is View)
                        {
                            ((View)mParent).clearFocusedInCluster();
                        }
                    }
                    //clearAccessibilityFocus();
                }
                else if (oldWidth <= 0 || oldHeight <= 0)
                {
                    if (mParent != null && canTakeFocus())
                    {
                        mParent.focusableViewAvailable(this);
                    }
                }
            }
            //rebuildOutline();
        }

        /**
         * Used to indicate that the parent of this view should clear its caches. This functionality
         * is used to force the parent to rebuild its display list (when hardware-accelerated),
         * which is necessary when various parent-managed properties of the view change, such as
         * alpha, translationX/Y, scrollX/Y, scaleX/Y, and rotation/X/Y. This method only
         * clears the parent caches and does not causes an invalidate event.
         *
         * @hide
         */
        protected void invalidateParentCaches()
        {
            if (mParent is View)
            {
                ((View)mParent).mPrivateFlags |= PFLAG_INVALIDATED;
            }
        }

        /**
         * Assign a size and position to this view.
         *
         * This is called from layout.
         *
         * @param left Left position, relative to parent
         * @param top Top position, relative to parent
         * @param right Right position, relative to parent
         * @param bottom Bottom position, relative to parent
         * @return true if the new size and position are different than the
         *         previous ones
         * {@hide}
         */
        protected bool setFrame(int left, int top, int right, int bottom)
        {
            bool changed = false;

            if (mLeft != left || mRight != right || mTop != top || mBottom != bottom)
            {
                changed = true;

                // Remember our drawn bit
                int drawn = mPrivateFlags & PFLAG_DRAWN;

                int oldWidth = mRight - mLeft;
                int oldHeight = mBottom - mTop;
                int newWidth = right - left;
                int newHeight = bottom - top;
                bool sizeChanged = (newWidth != oldWidth) || (newHeight != oldHeight);

                // Invalidate our old position
                invalidate(sizeChanged);

                mLeft = left;
                mTop = top;
                mRight = right;
                mBottom = bottom;

                mPrivateFlags |= PFLAG_HAS_BOUNDS;


                if (sizeChanged)
                {
                    sizeChange(newWidth, newHeight, oldWidth, oldHeight);
                }

                if ((mViewFlags & VISIBILITY_MASK) == VISIBLE /*|| mGhostView != null*/)
                {
                    // If we are visible, force the DRAWN bit to on so that
                    // this invalidate will go through (at least to our parent).
                    // This is because someone may have invalidated this view
                    // before this call to setFrame came in, thereby clearing
                    // the DRAWN bit.
                    mPrivateFlags |= PFLAG_DRAWN;
                    invalidate(sizeChanged);
                    // parent display list may need to be recreated based on a change in the bounds
                    // of any child
                    invalidateParentCaches();
                }

                // Reset drawn bit to original value (invalidate turns it off)
                mPrivateFlags |= drawn;

                //mBackgroundSizeChanged = true;
                //mDefaultFocusHighlightSizeChanged = true;
                //if (mForegroundInfo != null)
                //{
                //    mForegroundInfo.mBoundsChanged = true;
                //}

                //notifySubtreeAccessibilityStateChangedIfNeeded();
            }
            return changed;
        }

        /**
         * Invalidate the whole view. If the view is visible,
         * {@link #onDraw(android.graphics.Canvas)} will be called at some point in
         * the future.
         * <p>
         * This must be called from a UI thread. To call from a non-UI thread, call
         * {@link #postInvalidate()}.
         */
        public void invalidate()
        {
            invalidate(true);
        }

        /**
         * Mark the area defined by the rect (l,t,r,b) as needing to be drawn. The
         * coordinates of the dirty rect are relative to the view. If the view is
         * visible, {@link #onDraw(android.graphics.Canvas)} will be called at some
         * point in the future.
         * <p>
         * This must be called from a UI thread. To call from a non-UI thread, call
         * {@link #postInvalidate()}.
         *
         * @param l the left position of the dirty region
         * @param t the top position of the dirty region
         * @param r the right position of the dirty region
         * @param b the bottom position of the dirty region
         *
         * @deprecated The switch to hardware accelerated rendering in API 14 reduced
         * the importance of the dirty rectangle. In API 21 the given rectangle is
         * ignored entirely in favor of an internally-calculated area instead.
         * Because of this, clients are encouraged to just call {@link #invalidate()}.
         */
        public void invalidate(int l, int t, int r, int b)
        {
            int scrollX = mScrollX;
            int scrollY = mScrollY;
            invalidateInternal(l - scrollX, t - scrollY, r - scrollX, b - scrollY, true, false);
        }

        /**
         * This is where the invalidate() work actually happens. A full invalidate()
         * causes the drawing cache to be invalidated, but this function can be
         * called with invalidateCache set to false to skip that invalidation step
         * for cases that do not need it (for example, a component that remains at
         * the same dimensions with the same content).
         *
         * @param invalidateCache Whether the drawing cache for this view should be
         *            invalidated as well. This is usually true for a full
         *            invalidate, but may be set to false if the View's contents or
         *            dimensions have not changed.
         * @hide
         */
        public void invalidate(bool invalidateCache)
        {
            invalidateInternal(0, 0, mRight - mLeft, mBottom - mTop, invalidateCache, true);
        }

        /**
         * Do not invalidate views which are not visible and which are not running an animation. They
         * will not get drawn and they should not set dirty flags as if they will be drawn
         */
        private bool skipInvalidate()
        {
            return (mViewFlags & VISIBILITY_MASK) != VISIBLE
                //&& mCurrentAnimation == null
                && (
                    !(mParent is View)
                //|| !((View)Parent).isViewTransitioning(this)
                );
        }

        /**
         * True if this view has changed since the last time being drawn.
         *
         * @return The dirty state of this view.
         */
        public bool isDirty()
        {
            return (mPrivateFlags & PFLAG_DIRTY_MASK) != 0;
        }

        void invalidateInternal(int l, int t, int r, int b, bool invalidateCache,
                bool fullInvalidate)
        {
            //if (mGhostView != null)
            //{
            //mGhostView.invalidate(true);
            //return;
            //}

            if (skipInvalidate())
            {
                return;
            }

            // Reset content capture caches
            //mPrivateFlags4 &= ~PFLAG4_CONTENT_CAPTURE_IMPORTANCE_MASK;
            //mContentCaptureSessionCached = false;

            if ((mPrivateFlags & (PFLAG_DRAWN | PFLAG_HAS_BOUNDS)) == (PFLAG_DRAWN | PFLAG_HAS_BOUNDS)
                    || (invalidateCache && (mPrivateFlags & PFLAG_DRAWING_CACHE_VALID) == PFLAG_DRAWING_CACHE_VALID)
                    || (mPrivateFlags & PFLAG_INVALIDATED) != PFLAG_INVALIDATED
                    || (fullInvalidate
                    //&& isOpaque() != mLastIsOpaque
                    ))
            {
                if (fullInvalidate)
                {
                    //mLastIsOpaque = isOpaque();
                    mPrivateFlags &= ~PFLAG_DRAWN;
                }

                mPrivateFlags |= PFLAG_DIRTY;

                if (invalidateCache)
                {
                    mPrivateFlags |= PFLAG_INVALIDATED;
                    mPrivateFlags &= ~PFLAG_DRAWING_CACHE_VALID;
                }

                // Propagate the damage rectangle to the parent view.
                //AttachInfo ai = mAttachInfo;
                //Parent p = Parent;
                //if (p != null && ai != null && l < r && t < b)
                //{
                //Rect damage = ai.mTmpInvalRect;
                //damage.set(l, t, r, b);
                //p.invalidateChild(this, damage);
                //}

                // Damage the entire projection receiver, if necessary.
                //if (mBackground != null && mBackground.isProjected())
                //{
                //    View receiver = getProjectionReceiver();
                //    if (receiver != null)
                //    {
                //        receiver.damageInParent();
                //    }
                //}
            }
        }

        private SKRect rect;

        public static int FloatToPixel(float pixelF) => (int)Math.Ceiling(pixelF);

        /**
         * Get the LayoutParams associated with this view. All views should have
         * layout parameters. These supply parameters to the <i>parent</i> of this
         * view specifying how it should be arranged. There are many subclasses of
         * View.LayoutParams, and these correspond to the different subclasses
         * of View that are responsible for arranging their children.
         *
         * This method may return null if this View is not attached to a parent
         * View or {@link #setLayoutParams(android.view.View.LayoutParams)}
         * was not invoked successfully. When a View is attached to a parent
         * View, this method must not return null.
         *
         * @return The LayoutParams associated with this view, or null if no
         *         parameters have been set yet
         */
        public View.LayoutParams getLayoutParams()
        {
            return mLayoutParams;
        }

        /**
         * Set the layout parameters associated with this view. These supply
         * parameters to the <i>parent</i> of this view specifying how it should be
         * arranged. There are many subclasses of View.LayoutParams, and these
         * correspond to the different subclasses of View that are responsible
         * for arranging their children.
         *
         * @param params The layout parameters for this view, cannot be null
         */
        public void setLayoutParams(View.LayoutParams layout_params)
        {
            if (layout_params == null)
            {
                throw new Exception("Layout parameters cannot be null");
            }
            mLayoutParams = layout_params;
            resolveLayoutParams();
            if (mParent is View)
            {
                ((View)mParent).onSetLayoutParams(this, layout_params);
            }
            requestLayout();
        }

        /** @hide */
        protected void onSetLayoutParams(View child, LayoutParams layoutParams)
        {
            requestLayout();
        }

        /**
         * Resolve the layout parameters depending on the resolved layout direction
         *
         * @hide
         */
        public void resolveLayoutParams()
        {
            if (mLayoutParams != null)
            {
                //mLayoutParams.resolveLayoutDirection(getLayoutDirection());
            }
        }

        /**
         * Called from layout when this view should
         * assign a size and position to each of its children.
         *
         * Derived classes with children should override
         * this method and call layout on each of
         * their children.
         * @param changed This is a new size or position for this view
         * @param left Left position, relative to parent
         * @param top Top position, relative to parent
         * @param right Right position, relative to parent
         * @param bottom Bottom position, relative to parent
         */
        protected virtual void onLayout(bool changed, int l, int t, int r, int b)
        {
            int count = getChildCount();

            for (int i = 0; i < count; i++)
            {
                View child = getChildAt(i);
                if (child.getVisibility() != GONE)
                {

                    LayoutParams lp = child.getLayoutParams();

                    int childLeft = mPaddingLeft;
                    int childTop = mPaddingTop;
                    child.layout(childLeft, childTop,
                            childLeft + child.getMeasuredWidth(),
                            childTop + child.getMeasuredHeight());
                }
            }
        }

        private static SKPaint sDebugPaint;

        static SKPaint getDebugPaint()
        {
            if (sDebugPaint == null)
            {
                sDebugPaint = new SKPaint();
                sDebugPaint.IsAntialias = false;
            }
            return sDebugPaint;
        }

        int dipsToPixels(int dips)
        {
            float scale = Plugin.ScreenDensityAsFloat; //getContext().getResources().getDisplayMetrics().density;
            return (int)(dips * scale + 0.5f);
        }

        private static readonly SKColor DEBUG_CORNERS_COLOR = new SKColor(63, 127, 255);

        internal const int DEBUG_CORNERS_SIZE_DIP = 8;

        /**
         * The computed left padding in pixels that is used for drawing. This is the distance in
         * pixels between the left edge of this view and the left edge of its content.
         * {@hide}
         */
        protected int mPaddingLeft = 0;

        /**
         * The computed right padding in pixels that is used for drawing. This is the distance in
         * pixels between the right edge of this view and the right edge of its content.
         * {@hide}
         */
        protected int mPaddingRight = 0;

        /**
         * The computed top padding in pixels that is used for drawing. This is the distance in
         * pixels between the top edge of this view and the top edge of its content.
         * {@hide}
         */
        protected int mPaddingTop;

        /**
         * The computed bottom padding in pixels that is used for drawing. This is the distance in
         * pixels between the bottom edge of this view and the bottom edge of its content.
         * {@hide}
         */
        protected int mPaddingBottom;

        /**
         * If the View draws content inside its padding and enables fading edges,
         * it needs to support padding offsets. Padding offsets are added to the
         * fading edges to extend the length of the fade so that it covers pixels
         * drawn inside the padding.
         *
         * Subclasses of this class should override this method if they need
         * to draw content inside the padding.
         *
         * @return True if padding offset must be applied, false otherwise.
         *
         * @see #getLeftPaddingOffset()
         * @see #getRightPaddingOffset()
         * @see #getTopPaddingOffset()
         * @see #getBottomPaddingOffset()
         *
         * @since CURRENT
         */
        protected bool isPaddingOffsetRequired()
        {
            return false;
        }

        /**
         * Amount by which to extend the left fading region. Called only when
         * {@link #isPaddingOffsetRequired()} returns true.
         *
         * @return The left padding offset in pixels.
         *
         * @see #isPaddingOffsetRequired()
         *
         * @since CURRENT
         */
        protected int getLeftPaddingOffset()
        {
            return 0;
        }

        /**
         * Amount by which to extend the right fading region. Called only when
         * {@link #isPaddingOffsetRequired()} returns true.
         *
         * @return The right padding offset in pixels.
         *
         * @see #isPaddingOffsetRequired()
         *
         * @since CURRENT
         */
        protected int getRightPaddingOffset()
        {
            return 0;
        }

        /**
         * Amount by which to extend the top fading region. Called only when
         * {@link #isPaddingOffsetRequired()} returns true.
         *
         * @return The top padding offset in pixels.
         *
         * @see #isPaddingOffsetRequired()
         *
         * @since CURRENT
         */
        protected int getTopPaddingOffset()
        {
            return 0;
        }

        /**
         * Amount by which to extend the bottom fading region. Called only when
         * {@link #isPaddingOffsetRequired()} returns true.
         *
         * @return The bottom padding offset in pixels.
         *
         * @see #isPaddingOffsetRequired()
         *
         * @since CURRENT
         */
        protected int getBottomPaddingOffset()
        {
            return 0;
        }

        /**
         * @hide
         * @param offsetRequired
         */
        protected int getFadeTop(bool offsetRequired)
        {
            int top = mPaddingTop;
            if (offsetRequired) top += getTopPaddingOffset();
            return top;
        }

        /**
         * @hide
         * @param offsetRequired
         */
        protected int getFadeHeight(bool offsetRequired)
        {
            int padding = mPaddingTop;
            if (offsetRequired) padding += getTopPaddingOffset();
            return mBottom - mTop - mPaddingBottom - padding;
        }

        class Drawable { };
        class ScrollBarDrawable : Drawable
        {
            internal int getSize(bool v)
            {
                return 0;
            }
        }
        class Interpolator
        {
            public Interpolator(int v1, int v2)
            {
            }
        }
        class ViewConfiguration
        {
            internal int getScaledFadingEdgeLength()
            {
                throw new NotImplementedException();
            }

            internal int getScaledScrollBarSize()
            {
                throw new NotImplementedException();
            }

            internal int getScaledMinScrollbarTouchTarget()
            {
                throw new NotImplementedException();
            }

            internal static int getScrollDefaultDelay()
            {
                throw new NotImplementedException();
            }

            internal static int getScrollBarFadeDuration()
            {
                throw new NotImplementedException();
            }
        }

        /**
         * <p>ScrollabilityCache holds various fields used by a View when scrolling
         * is supported. This avoids keeping too many unused fields in most
         * instances of View.</p>
         */
        private class ScrollabilityCache : Runnable
        {

            /**
             * Scrollbars are not visible
             */
            public const int OFF = 0;

            /**
             * Scrollbars are visible
             */
            public const int ON = 1;

            /**
             * Scrollbars are fading away
             */
            public const int FADING = 2;

            public bool fadeScrollBars;

            public int fadingEdgeLength;
            public int scrollBarDefaultDelayBeforeFade;
            public int scrollBarFadeDuration;

            public int scrollBarSize;
            public int scrollBarMinTouchTarget;
            public ScrollBarDrawable scrollBar;
            public float[] interpolatorValues;
            public View host;

            public readonly SKPaint paint;
            public readonly SKMatrix matrix;
            public SKShader shader;

            public readonly Interpolator scrollBarInterpolator = new Interpolator(1, 2);

            private static readonly float[] OPAQUE = { 255 };
            private static readonly float[] TRANSPARENT = { 0.0f };

            /**
             * When fading should start. This time moves into the future every time
             * a new scroll happens. Measured based on SystemClock.uptimeMillis()
             */
            public long fadeStartTime;


            /**
             * The current state of the scrollbars: ON, OFF, or FADING
             */
            public int state = OFF;

            private int mLastColor;

            public readonly Rect mScrollBarBounds = new Rect();
            public readonly Rect mScrollBarTouchBounds = new Rect();

            public const int NOT_DRAGGING = 0;
            public const int DRAGGING_VERTICAL_SCROLL_BAR = 1;
            public const int DRAGGING_HORIZONTAL_SCROLL_BAR = 2;
            public int mScrollBarDraggingState = NOT_DRAGGING;

            public float mScrollBarDraggingPos = 0;

            public ScrollabilityCache(ViewConfiguration configuration, View host)
            {
                fadingEdgeLength = configuration.getScaledFadingEdgeLength();
                scrollBarSize = configuration.getScaledScrollBarSize();
                scrollBarMinTouchTarget = configuration.getScaledMinScrollbarTouchTarget();
                scrollBarDefaultDelayBeforeFade = ViewConfiguration.getScrollDefaultDelay();
                scrollBarFadeDuration = ViewConfiguration.getScrollBarFadeDuration();

                paint = new SKPaint();
                matrix = new SKMatrix();
                // use use a height of 1, and then wack the matrix each time we
                // actually use it.
                shader = SKShader.CreateLinearGradient(
                    new SKPoint(0, 0),
                    new SKPoint(0, 1),
                    new SKColor[] { 0xFF000000, 0 },
                    SKShaderTileMode.Clamp
                );
                paint.Shader = shader;
                paint.BlendMode = SKBlendMode.DstOut;
                this.host = host;
            }

            public void setFadeColor(int color)
            {
                if (color != mLastColor)
                {
                    mLastColor = color;

                    if (color != 0)
                    {
                        shader = SKShader.CreateLinearGradient(
                            new SKPoint(0, 0),
                            new SKPoint(0, 1),
                            new SKColor[] { (uint)color | 0xFF000000, (uint)color & 0x00FFFFFF },
                            SKShaderTileMode.Clamp
                        );
                        paint.Shader = shader;
                        // Restore the default transfer mode (src_over)
                        paint.SetToDefaultBlendMode();
                    }
                    else
                    {
                        shader = SKShader.CreateLinearGradient(
                            new SKPoint(0, 0),
                            new SKPoint(0, 1),
                            new SKColor[] { 0xFF000000, 0 },
                            SKShaderTileMode.Clamp
                        );
                        paint.Shader = shader;
                        paint.BlendMode = SKBlendMode.DstOut;
                    }
                }
            }

            public void run()
            {
                long now = 0; //AnimationUtils.currentAnimationTimeMillis();
                if (now >= fadeStartTime)
                {

                    // the animation fades the scrollbars out by changing
                    // the opacity (alpha) from fully opaque to fully
                    // transparent
                    int nextFrame = (int)now;
                    int framesCount = 0;

                    Interpolator interpolator = scrollBarInterpolator;

                    // Start opaque
                    //interpolator.setKeyFrame(framesCount++, nextFrame, OPAQUE);

                    // End transparent
                    nextFrame += scrollBarFadeDuration;
                    //interpolator.setKeyFrame(framesCount, nextFrame, TRANSPARENT);

                    state = FADING;

                    // Kick off the fade animation
                    host.invalidate(true);
                }
            }
        }

        private ScrollabilityCache mScrollCache;

        /**
         * Returns the strength, or intensity, of the top faded edge. The strength is
         * a value between 0.0 (no fade) and 1.0 (full fade). The default implementation
         * returns 0.0 or 1.0 but no value in between.
         *
         * Subclasses should override this method to provide a smoother fade transition
         * when scrolling occurs.
         *
         * @return the intensity of the top fade as a float between 0.0f and 1.0f
         */
        protected float getTopFadingEdgeStrength()
        {
            return computeVerticalScrollOffset() > 0 ? 1.0f : 0.0f;
        }

        /**
         * Returns the strength, or intensity, of the bottom faded edge. The strength is
         * a value between 0.0 (no fade) and 1.0 (full fade). The default implementation
         * returns 0.0 or 1.0 but no value in between.
         *
         * Subclasses should override this method to provide a smoother fade transition
         * when scrolling occurs.
         *
         * @return the intensity of the bottom fade as a float between 0.0f and 1.0f
         */
        protected float getBottomFadingEdgeStrength()
        {
            return computeVerticalScrollOffset() + computeVerticalScrollExtent() <
                    computeVerticalScrollRange() ? 1.0f : 0.0f;
        }

        /**
         * Returns the strength, or intensity, of the left faded edge. The strength is
         * a value between 0.0 (no fade) and 1.0 (full fade). The default implementation
         * returns 0.0 or 1.0 but no value in between.
         *
         * Subclasses should override this method to provide a smoother fade transition
         * when scrolling occurs.
         *
         * @return the intensity of the left fade as a float between 0.0f and 1.0f
         */
        protected float getLeftFadingEdgeStrength()
        {
            return computeHorizontalScrollOffset() > 0 ? 1.0f : 0.0f;
        }

        /**
         * Returns the strength, or intensity, of the right faded edge. The strength is
         * a value between 0.0 (no fade) and 1.0 (full fade). The default implementation
         * returns 0.0 or 1.0 but no value in between.
         *
         * Subclasses should override this method to provide a smoother fade transition
         * when scrolling occurs.
         *
         * @return the intensity of the right fade as a float between 0.0f and 1.0f
         */
        protected float getRightFadingEdgeStrength()
        {
            return computeHorizontalScrollOffset() + computeHorizontalScrollExtent() <
                    computeHorizontalScrollRange() ? 1.0f : 0.0f;
        }

        /**
         * <p>Compute the horizontal range that the horizontal scrollbar
         * represents.</p>
         *
         * <p>The range is expressed in arbitrary units that must be the same as the
         * units used by {@link #computeHorizontalScrollExtent()} and
         * {@link #computeHorizontalScrollOffset()}.</p>
         *
         * <p>The default range is the drawing width of this view.</p>
         *
         * @return the total horizontal range represented by the horizontal
         *         scrollbar
         *
         * @see #computeHorizontalScrollExtent()
         * @see #computeHorizontalScrollOffset()
         */
        protected int computeHorizontalScrollRange()
        {
            return getWidth();
        }

        /**
         * <p>Compute the horizontal offset of the horizontal scrollbar's thumb
         * within the horizontal range. This value is used to compute the position
         * of the thumb within the scrollbar's track.</p>
         *
         * <p>The range is expressed in arbitrary units that must be the same as the
         * units used by {@link #computeHorizontalScrollRange()} and
         * {@link #computeHorizontalScrollExtent()}.</p>
         *
         * <p>The default offset is the scroll offset of this view.</p>
         *
         * @return the horizontal offset of the scrollbar's thumb
         *
         * @see #computeHorizontalScrollRange()
         * @see #computeHorizontalScrollExtent()
         */
        protected int computeHorizontalScrollOffset()
        {
            return mScrollX;
        }

        /**
         * <p>Compute the horizontal extent of the horizontal scrollbar's thumb
         * within the horizontal range. This value is used to compute the length
         * of the thumb within the scrollbar's track.</p>
         *
         * <p>The range is expressed in arbitrary units that must be the same as the
         * units used by {@link #computeHorizontalScrollRange()} and
         * {@link #computeHorizontalScrollOffset()}.</p>
         *
         * <p>The default extent is the drawing width of this view.</p>
         *
         * @return the horizontal extent of the scrollbar's thumb
         *
         * @see #computeHorizontalScrollRange()
         * @see #computeHorizontalScrollOffset()
         */
        protected int computeHorizontalScrollExtent()
        {
            return getWidth();
        }

        /**
         * <p>Compute the vertical range that the vertical scrollbar represents.</p>
         *
         * <p>The range is expressed in arbitrary units that must be the same as the
         * units used by {@link #computeVerticalScrollExtent()} and
         * {@link #computeVerticalScrollOffset()}.</p>
         *
         * @return the total vertical range represented by the vertical scrollbar
         *
         * <p>The default range is the drawing height of this view.</p>
         *
         * @see #computeVerticalScrollExtent()
         * @see #computeVerticalScrollOffset()
         */
        protected int computeVerticalScrollRange()
        {
            return getHeight();
        }

        /**
         * <p>Compute the vertical offset of the vertical scrollbar's thumb
         * within the horizontal range. This value is used to compute the position
         * of the thumb within the scrollbar's track.</p>
         *
         * <p>The range is expressed in arbitrary units that must be the same as the
         * units used by {@link #computeVerticalScrollRange()} and
         * {@link #computeVerticalScrollExtent()}.</p>
         *
         * <p>The default offset is the scroll offset of this view.</p>
         *
         * @return the vertical offset of the scrollbar's thumb
         *
         * @see #computeVerticalScrollRange()
         * @see #computeVerticalScrollExtent()
         */
        protected int computeVerticalScrollOffset()
        {
            return mScrollY;
        }

        /**
         * <p>Compute the vertical extent of the vertical scrollbar's thumb
         * within the vertical range. This value is used to compute the length
         * of the thumb within the scrollbar's track.</p>
         *
         * <p>The range is expressed in arbitrary units that must be the same as the
         * units used by {@link #computeVerticalScrollRange()} and
         * {@link #computeVerticalScrollOffset()}.</p>
         *
         * <p>The default extent is the drawing height of this view.</p>
         *
         * @return the vertical extent of the scrollbar's thumb
         *
         * @see #computeVerticalScrollRange()
         * @see #computeVerticalScrollOffset()
         */
        protected int computeVerticalScrollExtent()
        {
            return getHeight();
        }

        /**
         * Check if this view can be scrolled horizontally in a certain direction.
         *
         * @param direction Negative to check scrolling left, positive to check scrolling right.
         * @return true if this view can be scrolled in the specified direction, false otherwise.
         */
        public bool canScrollHorizontally(int direction)
        {
            int offset = computeHorizontalScrollOffset();
            int range = computeHorizontalScrollRange() - computeHorizontalScrollExtent();
            if (range == 0) return false;
            if (direction < 0)
            {
                return offset > 0;
            }
            else
            {
                return offset < range - 1;
            }
        }

        /**
         * Check if this view can be scrolled vertically in a certain direction.
         *
         * @param direction Negative to check scrolling up, positive to check scrolling down.
         * @return true if this view can be scrolled in the specified direction, false otherwise.
         */
        public bool canScrollVertically(int direction)
        {
            int offset = computeVerticalScrollOffset();
            int range = computeVerticalScrollRange() - computeVerticalScrollExtent();
            if (range == 0) return false;
            if (direction < 0)
            {
                return offset > 0;
            }
            else
            {
                return offset < range - 1;
            }
        }

        void getScrollIndicatorBounds(Rect out_)
        {
            out_.left = mScrollX;
            out_.right = mScrollX + mRight - mLeft;
            out_.top = mScrollY;
            out_.bottom = mScrollY + mBottom - mTop;
        }

        private void debugDrawFocus(SKCanvas canvas)
        {
            if (isFocused())
            {
                int cornerSquareSize = dipsToPixels(DEBUG_CORNERS_SIZE_DIP);
                int l = mScrollX;
                int r = l + mRight - mLeft;
                int t = mScrollY;
                int b = t + mBottom - mTop;

                SKPaint paint = getDebugPaint();
                paint.SetColor(DEBUG_CORNERS_COLOR);

                // Draw squares in corners.
                paint.Style = SKPaintStyle.Fill;

                canvas.DrawRect(l, t, l + cornerSquareSize, t + cornerSquareSize, paint);
                canvas.DrawRect(r - cornerSquareSize, t, r, t + cornerSquareSize, paint);
                canvas.DrawRect(l, b - cornerSquareSize, l + cornerSquareSize, b, paint);
                canvas.DrawRect(r - cornerSquareSize, b - cornerSquareSize, r, b, paint);

                // Draw big X across the view.
                paint.Style = SKPaintStyle.Stroke;
                canvas.DrawLine(l, t, r, b, paint);
                canvas.DrawLine(l, b, r, t, paint);
            }
        }

        /**
         * Override this if your view is known to always be drawn on top of a solid color background,
         * and needs to draw fading edges. Returning a non-zero color enables the view system to
         * optimize the drawing of the fading edges. If you do return a non-zero color, the alpha
         * should be set to 0xFF.
         *
         * @see #setVerticalFadingEdgeEnabled(bool)
         * @see #setHorizontalFadingEdgeEnabled(bool)
         *
         * @return The known solid color background for this view, or 0 if the color may vary
         */
        public int getSolidColor()
        {
            return 0;
        }





        SKPicture mRenderNode;

        /**
         * This method is used to cause children of this View to restore or recreate their
         * display lists. It is called by getDisplayList() when the parent View does not need
         * to recreate its own display list, which would happen if it went through the normal
         * draw/dispatchDraw mechanisms.
         *
         * @hide
         */
        protected void dispatchGetDisplayList()
        {
            int count = mChildrenCount;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                View child = children[i];
                if (((child.mViewFlags & VISIBILITY_MASK) == VISIBLE
                    //|| child.getAnimation() != null
                    ))
                {
                    recreateChildDisplayList(child);
                }
            }
            //int transientCount = mTransientViews == null ? 0 : mTransientIndices.size();
            //for (int i = 0; i < transientCount; ++i)
            //{
            //    View child = mTransientViews.get(i);
            //    if (((child.mViewFlags & VISIBILITY_MASK) == VISIBLE || child.getAnimation() != null))
            //    {
            //        recreateChildDisplayList(child);
            //    }
            //}
            //if (mOverlay != null)
            //{
            //    View overlayView = mOverlay.getOverlayView();
            //    recreateChildDisplayList(overlayView);
            //}
            //if (mDisappearingChildren != null)
            //{
            //    ArrayList<View> disappearingChildren = mDisappearingChildren;
            //    int disappearingCount = disappearingChildren.size();
            //    for (int i = 0; i < disappearingCount; ++i)
            //    {
            //        View child = disappearingChildren.get(i);
            //        recreateChildDisplayList(child);
            //    }
            //}
        }

        private void recreateChildDisplayList(View child)
        {
            child.mRecreateDisplayList = (child.mPrivateFlags & PFLAG_INVALIDATED) != 0;
            child.mPrivateFlags &= ~PFLAG_INVALIDATED;
            child.updateDisplayListIfDirty();
            child.mRecreateDisplayList = false;
        }

        /**
         * Gets the RenderNode for the view, and updates its DisplayList (if needed and supported)
         * @hide
         */
        public SKPicture updateDisplayListIfDirty()
        {
            //if (!canHaveDisplayList())
            //{
            //    // can't populate RenderNode, don't try
            //    return renderNode;
            //}

            if ((mPrivateFlags & PFLAG_DRAWING_CACHE_VALID) == 0
                    //|| !renderNode.hasDisplayList()
                    || (mRecreateDisplayList))
            {
                // Don't need to recreate the display list, just need to tell our
                // children to restore/recreate theirs
                if (
                    //renderNode.hasDisplayList() &&
                    !mRecreateDisplayList)
                {
                    mPrivateFlags |= PFLAG_DRAWN | PFLAG_DRAWING_CACHE_VALID;
                    mPrivateFlags &= ~PFLAG_DIRTY_MASK;
                    dispatchGetDisplayList();

                    return mRenderNode; // no work needed
                }

                // If we got here, we're recreating it. Mark it as such to ensure that
                // we copy in child display lists into ours in drawChild()
                mRecreateDisplayList = true;

                int width = mRight - mLeft;
                int height = mBottom - mTop;
                //int layerType = getLayerType();

                // Hacky hack: Reset any stretch effects as those are applied during the draw pass
                // instead of being "stateful" like other RenderNode properties
                //renderNode.clearStretch();

                if (mRenderNode != null) mRenderNode.Dispose();
                SKPictureRecorder pictureRecorder = new SKPictureRecorder();

                //SKCanvas canvas = mAttachInfo.mViewRootImpl.drawingCanvas;
                SKCanvas canvas = pictureRecorder.BeginRecording(new SKRect(0, 0, width, height));
                canvas.setIsHardwareAccelerated(true);
                canvas.setWidthHeight(width, height);

                try
                {
                    //if (layerType == LAYER_TYPE_SOFTWARE)
                    //{
                    //    buildDrawingCache(true);
                    //    Bitmap cache = getDrawingCache(true);
                    //    if (cache != null)
                    //    {
                    //        canvas.drawBitmap(cache, 0, 0, mLayerPaint);
                    //    }
                    //}
                    //else
                    //{
                    //computeScroll();
                    int r = canvas.Save();
                    canvas.Translate(getX(), getY());

                    canvas.Translate(-mScrollX, -mScrollY);

                    mPrivateFlags |= PFLAG_DRAWN | PFLAG_DRAWING_CACHE_VALID;
                    mPrivateFlags &= ~PFLAG_DIRTY_MASK;

                    // Fast path for layouts with no backgrounds
                    if ((mPrivateFlags & PFLAG_SKIP_DRAW) == PFLAG_SKIP_DRAW)
                    {
                        dispatchDraw(canvas);
                        //drawAutofilledHighlight(canvas);
                        //if (mOverlay != null && !mOverlay.isEmpty())
                        //{
                        //    mOverlay.getOverlayView().draw(canvas);
                        //}
                        if (isShowingLayoutBounds())
                        {
                            debugDrawFocus(canvas);
                        }
                    }
                    else
                    {
                        draw(canvas);
                    }
                    canvas.RestoreToCount(r);
                    //}
                }
                finally
                {
                    mRenderNode = pictureRecorder.EndRecording();
                    setDisplayListProperties(mRenderNode);
                }
            }
            else
            {
                mPrivateFlags |= PFLAG_DRAWN | PFLAG_DRAWING_CACHE_VALID;
                mPrivateFlags &= ~PFLAG_DIRTY_MASK;
            }
            return mRenderNode;
        }

        /**
         * This method is called by getDisplayList() when a display list is recorded for a View.
         * It pushes any properties to the RenderNode that aren't managed by the RenderNode.
         */
        void setDisplayListProperties(SKPicture renderNode)
        {
            if (renderNode != null)
            {
                //renderNode.setHasOverlappingRendering(getHasOverlappingRendering());
                //renderNode.setClipToBounds(mParent is View
                //        && ((View)mParent).getClipChildren());

                //float alpha = 1;
                //if (mParent is View && (((View)mParent).mGroupFlags &
                //        FLAG_SUPPORT_STATIC_TRANSFORMATIONS) != 0) {
                //    View parentVG = (View)mParent;
                //    Transformation t = parentVG.getChildTransformation();
                //    if (parentVG.getChildStaticTransformation(this, t))
                //    {
                //        int transformType = t.getTransformationType();
                //        if (transformType != Transformation.TYPE_IDENTITY)
                //        {
                //            if ((transformType & Transformation.TYPE_ALPHA) != 0)
                //            {
                //                alpha = t.getAlpha();
                //            }
                //            if ((transformType & Transformation.TYPE_MATRIX) != 0)
                //            {
                //                renderNode.setStaticMatrix(t.getMatrix());
                //            }
                //        }
                //    }
                //}
                //if (mTransformationInfo != null)
                //{
                //    alpha *= getFinalAlpha();
                //    if (alpha < 1)
                //    {
                //        int multipliedAlpha = (int)(255 * alpha);
                //        if (onSetAlpha(multipliedAlpha))
                //        {
                //            alpha = 1;
                //        }
                //    }
                //    renderNode.setAlpha(alpha);
                //}
                //else if (alpha < 1)
                //{
                //    renderNode.setAlpha(alpha);
                //}
            }
        }

        /**
         * Manually render this view (and all of its children) to the given Canvas.
         * The view must have already done a full layout before this function is
         * called.  When implementing a view, implement
         * {@link #onDraw(android.graphics.Canvas)} instead of overriding this method.
         * If you do need to override this method, call the superclass version.
         *
         * @param canvas The Canvas to which the View is rendered.
         */
        public void draw(SKCanvas canvas)
        {
            int privateFlags = mPrivateFlags;
            mPrivateFlags = (privateFlags & ~PFLAG_DIRTY_MASK) | PFLAG_DRAWN;

            /*
             * Draw traversal performs several drawing steps which must be executed
             * in the appropriate order:
             *
             *      1. Draw the background
             *      2. If necessary, save the canvas' layers to prepare for fading
             *      3. Draw view's content
             *      4. Draw children
             *      5. If necessary, draw the fading edges and restore layers
             *      6. Draw decorations (scrollbars for instance)
             *      7. If necessary, draw the default focus highlight
             */

            // Step 1, draw the background, if needed
            int saveCount;

            //drawBackground(canvas);

            // skip step 2 & 5 if possible (common case)
            int viewFlags = mViewFlags;
            bool horizontalEdges = (viewFlags & FADING_EDGE_HORIZONTAL) != 0;
            bool verticalEdges = (viewFlags & FADING_EDGE_VERTICAL) != 0;
            if (!verticalEdges && !horizontalEdges)
            {
                // Step 3, draw the content
                onDraw(canvas);

                // Step 4, draw the children
                dispatchDraw(canvas);

                //drawAutofilledHighlight(canvas);

                // Overlay is part of the content and draws beneath Foreground
                //if (mOverlay != null && !mOverlay.isEmpty())
                //{
                //mOverlay.getOverlayView().dispatchDraw(canvas);
                //}

                // Step 6, draw decorations (foreground, scrollbars)
                //onDrawForeground(canvas);

                // Step 7, draw the default focus highlight
                //drawDefaultFocusHighlight(canvas);

                if (isShowingLayoutBounds())
                {
                    debugDrawFocus(canvas);
                }

                // we're done...
                return;
            }
            Console.WriteLine("EDGE FADING IS NOT SUPPORTED");

            /*
             * Here we do the full fledged routine...
             * (this is an uncommon case where speed matters less,
             * this is why we repeat some of the tests that have been
             * done above)
             */

            //bool drawTop = false;
            //bool drawBottom = false;
            //bool drawLeft = false;
            //bool drawRight = false;

            //float topFadeStrength = 0.0f;
            //float bottomFadeStrength = 0.0f;
            //float leftFadeStrength = 0.0f;
            //float rightFadeStrength = 0.0f;

            //// Step 2, save the canvas' layers
            //int paddingLeft = mPaddingLeft;

            //bool offsetRequired = isPaddingOffsetRequired();
            //if (offsetRequired)
            //{
            //    paddingLeft += getLeftPaddingOffset();
            //}

            //int left = mScrollX + paddingLeft;
            //int right = left + mRight - mLeft - mPaddingRight - paddingLeft;
            //int top = mScrollY + getFadeTop(offsetRequired);
            //int bottom = top + getFadeHeight(offsetRequired);

            //if (offsetRequired)
            //{
            //    right += getRightPaddingOffset();
            //    bottom += getBottomPaddingOffset();
            //}

            //ScrollabilityCache scrollabilityCache = mScrollCache;
            //float fadeHeight = scrollabilityCache.fadingEdgeLength;
            //int length = (int)fadeHeight;

            //// clip the fade length if top and bottom fades overlap
            //// overlapping fades produce odd-looking artifacts
            //if (verticalEdges && (top + length > bottom - length))
            //{
            //    length = (bottom - top) / 2;
            //}

            //// also clip horizontal fades if necessary
            //if (horizontalEdges && (left + length > right - length))
            //{
            //    length = (right - left) / 2;
            //}

            //if (verticalEdges)
            //{
            //    topFadeStrength = Math.Max(0.0f, Math.Min(1.0f, getTopFadingEdgeStrength()));
            //    drawTop = topFadeStrength * fadeHeight > 1.0f;
            //    bottomFadeStrength = Math.Max(0.0f, Math.Min(1.0f, getBottomFadingEdgeStrength()));
            //    drawBottom = bottomFadeStrength * fadeHeight > 1.0f;
            //}

            //if (horizontalEdges)
            //{
            //    leftFadeStrength = Math.Max(0.0f, Math.Min(1.0f, getLeftFadingEdgeStrength()));
            //    drawLeft = leftFadeStrength * fadeHeight > 1.0f;
            //    rightFadeStrength = Math.Max(0.0f, Math.Min(1.0f, getRightFadingEdgeStrength()));
            //    drawRight = rightFadeStrength * fadeHeight > 1.0f;
            //}

            //saveCount = canvas.SaveCount;
            //int topSaveCount = -1;
            //int bottomSaveCount = -1;
            //int leftSaveCount = -1;
            //int rightSaveCount = -1;

            //int solidColor = getSolidColor();
            //if (solidColor == 0)
            //{
            //    if (drawTop)
            //    {
            //        topSaveCount = canvas.saveUnclippedLayer(left, top, right, top + length);
            //    }

            //    if (drawBottom)
            //    {
            //        bottomSaveCount = canvas.saveUnclippedLayer(left, bottom - length, right, bottom);
            //    }

            //    if (drawLeft)
            //    {
            //        leftSaveCount = canvas.saveUnclippedLayer(left, top, left + length, bottom);
            //    }

            //    if (drawRight)
            //    {
            //        rightSaveCount = canvas.saveUnclippedLayer(right - length, top, right, bottom);
            //    }
            //}
            //else
            //{
            //    scrollabilityCache.setFadeColor(solidColor);
            //}

            //// Step 3, draw the content
            //OnDraw(canvas);

            //// Step 4, draw the children
            //dispatchDraw(canvas);

            //// Step 5, draw the fade effect and restore layers
            //SKPaint p = scrollabilityCache.paint;
            //SKMatrix matrix = scrollabilityCache.matrix;
            //SKShader fade = scrollabilityCache.shader;

            //// must be restored in the reverse order that they were saved
            //if (drawRight)
            //{
            //    matrix.setScale(1, fadeHeight * rightFadeStrength);
            //    matrix.postRotate(90);
            //    matrix.postTranslate(right, top);
            //    fade.setLocalMatrix(matrix);
            //    p.setShader(fade);
            //    if (solidColor == 0)
            //    {
            //        canvas.restoreUnclippedLayer(rightSaveCount, p);

            //    }
            //    else
            //    {
            //        canvas.drawRect(right - length, top, right, bottom, p);
            //    }
            //}

            //if (drawLeft)
            //{
            //    matrix.setScale(1, fadeHeight * leftFadeStrength);
            //    matrix.postRotate(-90);
            //    matrix.postTranslate(left, top);
            //    fade.setLocalMatrix(matrix);
            //    p.setShader(fade);
            //    if (solidColor == 0)
            //    {
            //        canvas.restoreUnclippedLayer(leftSaveCount, p);
            //    }
            //    else
            //    {
            //        canvas.drawRect(left, top, left + length, bottom, p);
            //    }
            //}

            //if (drawBottom)
            //{
            //    matrix.setScale(1, fadeHeight * bottomFadeStrength);
            //    matrix.postRotate(180);
            //    matrix.postTranslate(left, bottom);
            //    fade.setLocalMatrix(matrix);
            //    p.setShader(fade);
            //    if (solidColor == 0)
            //    {
            //        canvas.restoreUnclippedLayer(bottomSaveCount, p);
            //    }
            //    else
            //    {
            //        canvas.drawRect(left, bottom - length, right, bottom, p);
            //    }
            //}

            //if (drawTop)
            //{
            //    matrix.setScale(1, fadeHeight * topFadeStrength);
            //    matrix.postTranslate(left, top);
            //    fade.setLocalMatrix(matrix);
            //    p.setShader(fade);
            //    if (solidColor == 0)
            //    {
            //        canvas.restoreUnclippedLayer(topSaveCount, p);
            //    }
            //    else
            //    {
            //        canvas.drawRect(left, top, right, top + length, p);
            //    }
            //}

            //canvas.restoreToCount(saveCount);

            //drawAutofilledHighlight(canvas);

            //// Overlay is part of the content and draws beneath Foreground
            //if (mOverlay != null && !mOverlay.isEmpty())
            //{
            //    mOverlay.getOverlayView().dispatchDraw(canvas);
            //}

            //// Step 6, draw decorations (foreground, scrollbars)
            //onDrawForeground(canvas);

            //// Step 7, draw the default focus highlight
            //drawDefaultFocusHighlight(canvas);

            if (isShowingLayoutBounds())
            {
                debugDrawFocus(canvas);
            }
        }

        protected virtual void onDraw(SKCanvas canvas)
        {
        }

        public virtual void OnTouch(MultiTouch touch)
        {
        }

        /**
         * Return the width of your view.
         *
         * @return The width of your view, in pixels.
         */
        public int getWidth()
        {
            return mRight - mLeft;
        }

        /**
         * Return the height of your view.
         *
         * @return The height of your view, in pixels.
         */
        public int getHeight()
        {
            return mBottom - mTop;
        }

        /**
         * Find and return all touchable views that are descendants of this view,
         * possibly including this view if it is touchable itself.
         *
         * @return A list of touchable views
         */
        public List<View> getTouchables()
        {
            List<View> result = new List<View>();
            addTouchables(result);
            return result;
        }

        /**
         * Add any touchable views that are descendants of this view (possibly
         * including this view if it is touchable itself) to views.
         *
         * @param views Touchable views found so far
         */
        public void addTouchables(List<View> views)
        {
            int viewFlags = mViewFlags;

            if (((viewFlags & CLICKABLE) == CLICKABLE || (viewFlags & LONG_CLICKABLE) == LONG_CLICKABLE
                    || (viewFlags & CONTEXT_CLICKABLE) == CONTEXT_CLICKABLE)
                    && (viewFlags & ENABLED_MASK) == ENABLED)
            {
                views.Add(this);
            }
        }


        /**
         * Offset a rectangle that is in a descendant's coordinate
         * space into our coordinate space.
         * @param descendant A descendant of this view
         * @param rect A rectangle defined in descendant's coordinate space.
         */
        public void offsetDescendantRectToMyCoords(View descendant, Rect rect)
        {
            offsetRectBetweenParentAndChild(descendant, rect, true, false);
        }

        /**
         * Offset a rectangle that is in our coordinate space into an ancestor's
         * coordinate space.
         * @param descendant A descendant of this view
         * @param rect A rectangle defined in descendant's coordinate space.
         */
        public void offsetRectIntoDescendantCoords(View descendant, Rect rect)
        {
            offsetRectBetweenParentAndChild(descendant, rect, false, false);
        }

        /**
         * Helper method that offsets a rect either from parent to descendant or
         * descendant to parent.
         */
        public void offsetRectBetweenParentAndChild(View descendant, Rect rect,
                bool offsetFromChildToParent, bool clipToBounds)
        {

            // already in the same coord system :)
            if (descendant == this)
            {
                return;
            }

            Parent theParent = descendant.mParent;

            // search and offset up to the parent
            while ((theParent != null)
                    && (theParent is View)
                && (theParent != this))
            {

                if (offsetFromChildToParent)
                {
                    rect.offset(descendant.mLeft - descendant.mScrollX,
                            descendant.mTop - descendant.mScrollY);
                    if (clipToBounds)
                    {
                        View p = (View)theParent;
                        bool intersected = rect.intersect(0, 0, p.mRight - p.mLeft,
                                p.mBottom - p.mTop);
                        if (!intersected)
                        {
                            rect.setEmpty();
                        }
                    }
                }
                else
                {
                    if (clipToBounds)
                    {
                        View p = (View)theParent;
                        bool intersected = rect.intersect(0, 0, p.mRight - p.mLeft,
                                p.mBottom - p.mTop);
                        if (!intersected)
                        {
                            rect.setEmpty();
                        }
                    }
                    rect.offset(descendant.mScrollX - descendant.mLeft,
                            descendant.mScrollY - descendant.mTop);
                }

                descendant = (View)theParent;
                theParent = descendant.mParent;
            }

            // now that we are up to this view, need to offset one more time
            // to get into our coordinate space
            if (theParent == this)
            {
                if (offsetFromChildToParent)
                {
                    rect.offset(descendant.mLeft - descendant.mScrollX,
                            descendant.mTop - descendant.mScrollY);
                }
                else
                {
                    rect.offset(descendant.mScrollX - descendant.mLeft,
                            descendant.mScrollY - descendant.mTop);
                }
            }
            else
            {
                throw new Exception("parameter must be a descendant of this view");
            }
        }

        /*
         * Caller is responsible for calling requestLayout if necessary.
         * (This allows addViewInLayout to not request a new layout.)
         */
        void assignParent(Parent parent)
        {
            if (this.mParent == null)
            {
                this.mParent = parent;
            }
            else if (parent == null)
            {
                this.mParent = null;
            }
            else
            {
                throw new Exception("view " + this + " being added, but"
                        + " it already has a parent");
            }
        }

        /**
         * Returns whether this View should receive focus when the focus is restored for the view
         * hierarchy containing this view.
         * <p>
         * Focus gets restored for a view hierarchy when the root of the hierarchy gets added to a
         * window or serves as a target of cluster navigation.
         *
         * @see #restoreDefaultFocus()
         *
         * @return {@code true} if this view is the default-focus view, {@code false} otherwise
         * @attr ref android.R.styleable#View_focusedByDefault
         */
        public bool isFocusedByDefault()
        {
            return (mPrivateFlags & PFLAG_FOCUSED_BY_DEFAULT) != 0;
        }

        /**
         * Sets whether this View should receive focus when the focus is restored for the view
         * hierarchy containing this view.
         * <p>
         * Focus gets restored for a view hierarchy when the root of the hierarchy gets added to a
         * window or serves as a target of cluster navigation.
         *
         * @param isFocusedByDefault {@code true} to set this view as the default-focus view,
         *                           {@code false} otherwise.
         *
         * @see #restoreDefaultFocus()
         *
         * @attr ref android.R.styleable#View_focusedByDefault
         */
        public void setFocusedByDefault(bool isFocusedByDefault)
        {
            if (isFocusedByDefault == ((mPrivateFlags & PFLAG_FOCUSED_BY_DEFAULT) != 0))
            {
                return;
            }

            if (isFocusedByDefault)
            {
                mPrivateFlags |= PFLAG_FOCUSED_BY_DEFAULT;
            }
            else
            {
                mPrivateFlags &= ~PFLAG_FOCUSED_BY_DEFAULT;
            }

            if (mParent is View)
            {
                if (isFocusedByDefault)
                {
                    ((View)mParent).setDefaultFocus(this);
                }
                else
                {
                    ((View)mParent).clearDefaultFocus(this);
                }
            }
        }

        void setDefaultFocus(View child)
        {
            // Stop at any higher view which is explicitly focused-by-default
            if (mDefaultFocus != null && mDefaultFocus.isFocusedByDefault())
            {
                return;
            }

            mDefaultFocus = child;

            if (mParent is View)
            {
                ((View)mParent).setDefaultFocus(this);
            }
        }

        /**
         * Clears the default-focus chain from {@param child} up to the first parent which has another
         * default-focusable branch below it or until there is no default-focus chain.
         *
         * @param child
         */
        void clearDefaultFocus(View child)
        {
            // Stop at any higher view which is explicitly focused-by-default
            if (mDefaultFocus != child && mDefaultFocus != null
                    && mDefaultFocus.isFocusedByDefault())
            {
                return;
            }

            mDefaultFocus = null;

            // Search child siblings for default focusables.
            for (int i = 0; i < mChildrenCount; ++i)
            {
                View sibling = mChildren[i];
                if (sibling.isFocusedByDefault())
                {
                    mDefaultFocus = sibling;
                    return;
                }
                else if (mDefaultFocus == null && sibling.hasDefaultFocus())
                {
                    mDefaultFocus = sibling;
                }
            }

            if (mParent is View)
            {
                ((View)mParent).clearDefaultFocus(this);
            }
        }

        /**
         * Returns whether the view hierarchy with this view as a root contain a default-focus view.
         *
         * @return {@code true} if this view has default focus, {@code false} otherwise
         */
        bool hasDefaultFocus()
        {
            return mDefaultFocus != null || isFocusedByDefault();
        }


















        /**
         * LayoutParams are used by views to tell their parents how they want to be
         * laid out. See
         * {@link android.R.styleable#View_Layout View Layout Attributes}
         * for a list of all child view attributes that this class supports.
         *
         * <p>
         * The base LayoutParams class just describes how big the view wants to be
         * for both width and height. For each dimension, it can specify one of:
         * <ul>
         * <li>FILL_PARENT (renamed MATCH_PARENT in API Level 8 and higher), which
         * means that the view wants to be as big as its parent (minus padding)
         * <li> WRAP_CONTENT, which means that the view wants to be just big enough
         * to enclose its content (plus padding)
         * <li> an exact number
         * </ul>
         * There are subclasses of LayoutParams for different subclasses of
         * View. For example, AbsoluteLayout has its own subclass of
         * LayoutParams which adds an X and Y value.</p>
         *
         * <div class="special reference">
         * <h3>Developer Guides</h3>
         * <p>For more information about creating user interface layouts, read the
         * <a href="{@docRoot}guide/topics/ui/declaring-layout.html">XML Layouts</a> developer
         * guide.</p></div>
         *
         * @attr ref android.R.styleable#View_Layout_layout_height
         * @attr ref android.R.styleable#View_Layout_layout_width
         */
        public class LayoutParams
        {
            /**
             * Special value for the height or width requested by a View.
             * FILL_PARENT means that the view wants to be as big as its parent,
             * minus the parent's padding, if any. This value is deprecated
             * starting in API Level 8 and replaced by {@link #MATCH_PARENT}.
             */
            public const int FILL_PARENT = -1;

            /**
             * Special value for the height or width requested by a View.
             * MATCH_PARENT means that the view wants to be as big as its parent,
             * minus the parent's padding, if any. Introduced in API Level 8.
             */
            public const int MATCH_PARENT = -1;

            /**
             * Special value for the height or width requested by a View.
             * WRAP_CONTENT means that the view wants to be just large enough to fit
             * its own internal content, taking its own padding into account.
             */
            public const int WRAP_CONTENT = -2;

            /**
             * Information about how wide the view wants to be. Can be one of the
             * constants FILL_PARENT (replaced by MATCH_PARENT
             * in API Level 8) or WRAP_CONTENT, or an exact size.
             */
            public int width;

            /**
             * Information about how tall the view wants to be. Can be one of the
             * constants FILL_PARENT (replaced by MATCH_PARENT
             * in API Level 8) or WRAP_CONTENT, or an exact size.
             */
            public int height;

            /**
             * Used to animate layouts.
             */
            //public LayoutAnimationController.AnimationParameters layoutAnimationParameters;

            /**
             * Creates a new set of layout parameters with the specified width
             * and height.
             *
             * @param width the width, either {@link #WRAP_CONTENT},
             *        {@link #FILL_PARENT} (replaced by {@link #MATCH_PARENT} in
             *        API Level 8), or a fixed size in pixels
             * @param height the height, either {@link #WRAP_CONTENT},
             *        {@link #FILL_PARENT} (replaced by {@link #MATCH_PARENT} in
             *        API Level 8), or a fixed size in pixels
             */
            public LayoutParams(int width, int height)
            {
                this.width = width;
                this.height = height;
            }

            /**
             * Copy constructor. Clones the width and height values of the source.
             *
             * @param source The layout params to copy from.
             */
            public LayoutParams(LayoutParams source)
            {
                this.width = source.width;
                this.height = source.height;
            }

            /**
             * Used internally by MarginLayoutParams.
             * @hide
             */
            internal LayoutParams()
            {
            }

            /**
             * Resolve layout parameters depending on the layout direction. Subclasses that care about
             * layoutDirection changes should override this method. The default implementation does
             * nothing.
             *
             * @param layoutDirection the direction of the layout
             *
             * {@link View#LAYOUT_DIRECTION_LTR}
             * {@link View#LAYOUT_DIRECTION_RTL}
             */
            public virtual void resolveLayoutDirection(int layoutDirection)
            {
            }

            /**
             * Returns a String representation of this set of layout parameters.
             *
             * @param output the String to prepend to the internal representation
             * @return a String with the following format: output +
             *         "View.LayoutParams={ width=WIDTH, height=HEIGHT }"
             *
             * @hide
             */
            public virtual string debug(string output)
            {
                return output + "View.LayoutParams={ width="
                        + sizeToString(width) + ", height=" + sizeToString(height) + " }";
            }

            /**
             * Use {@code canvas} to draw suitable debugging annotations for these LayoutParameters.
             *
             * @param view the view that contains these layout parameters
             * @param canvas the canvas on which to draw
             *
             * @hide
             */
            public virtual void onDebugDraw(View view, SKCanvas canvas, SKPaint paint)
            {
            }

            /**
             * Converts the specified size to a readable String.
             *
             * @param size the size to convert
             * @return a String instance representing the supplied size
             *
             * @hide
             */
            protected static string sizeToString(int size)
            {
                if (size == WRAP_CONTENT)
                {
                    return "wrap-content";
                }
                if (size == MATCH_PARENT)
                {
                    return "match-parent";
                }
                return size.ToString();
            }

        }


        /**
         * Per-child layout information for layouts that support margins.
         * See
         * {@link android.R.styleable#View_MarginLayout View Margin Layout Attributes}
         * for a list of all child view attributes that this class supports.
         *
         * @attr ref android.R.styleable#View_MarginLayout_layout_margin
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginHorizontal
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginVertical
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginLeft
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginTop
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginRight
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginBottom
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginStart
         * @attr ref android.R.styleable#View_MarginLayout_layout_marginEnd
         */
        public class MarginLayoutParams : LayoutParams
        {
            /**
             * The left margin in pixels of the child. Margin values should be positive.
             * Call {@link View#setLayoutParams(LayoutParams)} after reassigning a new value
             * to this field.
             */
            public int leftMargin;

            /**
             * The top margin in pixels of the child. Margin values should be positive.
             * Call {@link View#setLayoutParams(LayoutParams)} after reassigning a new value
             * to this field.
             */
            public int topMargin;

            /**
             * The right margin in pixels of the child. Margin values should be positive.
             * Call {@link View#setLayoutParams(LayoutParams)} after reassigning a new value
             * to this field.
             */
            public int rightMargin;

            /**
             * The bottom margin in pixels of the child. Margin values should be positive.
             * Call {@link View#setLayoutParams(LayoutParams)} after reassigning a new value
             * to this field.
             */
            public int bottomMargin;

            /**
             * The start margin in pixels of the child. Margin values should be positive.
             * Call {@link View#setLayoutParams(LayoutParams)} after reassigning a new value
             * to this field.
             */
            private int startMargin = DEFAULT_MARGIN_RELATIVE;

            /**
             * The end margin in pixels of the child. Margin values should be positive.
             * Call {@link View#setLayoutParams(LayoutParams)} after reassigning a new value
             * to this field.
             */
            private int endMargin = DEFAULT_MARGIN_RELATIVE;

            /**
             * The default start and end margin.
             * @hide
             */
            public const int DEFAULT_MARGIN_RELATIVE = int.MinValue;

            /**
             * Bit  0: layout direction
             * Bit  1: layout direction
             * Bit  2: left margin undefined
             * Bit  3: right margin undefined
             * Bit  4: is RTL compatibility mode
             * Bit  5: need resolution
             *
             * Bit 6 to 7 not used
             *
             * @hide
             */
            int mMarginFlags;

            internal const int LAYOUT_DIRECTION_MASK = 0x00000003;
            internal const int LEFT_MARGIN_UNDEFINED_MASK = 0x00000004;
            internal const int RIGHT_MARGIN_UNDEFINED_MASK = 0x00000008;
            internal const int RTL_COMPATIBILITY_MODE_MASK = 0x00000010;
            internal const int NEED_RESOLUTION_MASK = 0x00000020;

            internal const int DEFAULT_MARGIN_RESOLVED = 0;
            internal const int UNDEFINED_MARGIN = DEFAULT_MARGIN_RELATIVE;

            public MarginLayoutParams(int width, int height) : base(width, height)
            {

                mMarginFlags |= LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= ~NEED_RESOLUTION_MASK;
                mMarginFlags &= ~RTL_COMPATIBILITY_MODE_MASK;
            }

            /**
             * Copy constructor. Clones the width, height and margin values of the source.
             *
             * @param source The layout params to copy from.
             */
            public MarginLayoutParams(MarginLayoutParams source) : base(source.width, source.height)
            {
                this.leftMargin = source.leftMargin;
                this.topMargin = source.topMargin;
                this.rightMargin = source.rightMargin;
                this.bottomMargin = source.bottomMargin;
                this.startMargin = source.startMargin;
                this.endMargin = source.endMargin;

                this.mMarginFlags = source.mMarginFlags;
            }

            public MarginLayoutParams(LayoutParams source) : base(source)
            {
                mMarginFlags |= LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags |= RIGHT_MARGIN_UNDEFINED_MASK;

                mMarginFlags &= ~NEED_RESOLUTION_MASK;
                mMarginFlags &= ~RTL_COMPATIBILITY_MODE_MASK;
            }

            /**
             * @hide Used internally.
             */
            public void copyMarginsFrom(MarginLayoutParams source)
            {
                this.leftMargin = source.leftMargin;
                this.topMargin = source.topMargin;
                this.rightMargin = source.rightMargin;
                this.bottomMargin = source.bottomMargin;
                this.startMargin = source.startMargin;
                this.endMargin = source.endMargin;

                this.mMarginFlags = source.mMarginFlags;
            }

            /**
             * Sets the margins, in pixels. A call to {@link android.view.View#requestLayout()} needs
             * to be done so that the new margins are taken into account. Left and right margins may be
             * overridden by {@link android.view.View#requestLayout()} depending on layout direction.
             * Margin values should be positive.
             *
             * @param left the left margin size
             * @param top the top margin size
             * @param right the right margin size
             * @param bottom the bottom margin size
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginLeft
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginTop
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginRight
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginBottom
             */
            public void setMargins(int left, int top, int right, int bottom)
            {
                leftMargin = left;
                topMargin = top;
                rightMargin = right;
                bottomMargin = bottom;
                mMarginFlags &= ~LEFT_MARGIN_UNDEFINED_MASK;
                mMarginFlags &= ~RIGHT_MARGIN_UNDEFINED_MASK;
                if (isMarginRelative())
                {
                    mMarginFlags |= NEED_RESOLUTION_MASK;
                }
                else
                {
                    mMarginFlags &= ~NEED_RESOLUTION_MASK;
                }
            }

            /**
             * Sets the relative margins, in pixels. A call to {@link android.view.View#requestLayout()}
             * needs to be done so that the new relative margins are taken into account. Left and right
             * margins may be overridden by {@link android.view.View#requestLayout()} depending on
             * layout direction. Margin values should be positive.
             *
             * @param start the start margin size
             * @param top the top margin size
             * @param end the right margin size
             * @param bottom the bottom margin size
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginStart
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginTop
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginEnd
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginBottom
             *
             * @hide
             */
            public void setMarginsRelative(int start, int top, int end, int bottom)
            {
                startMargin = start;
                topMargin = top;
                endMargin = end;
                bottomMargin = bottom;
                mMarginFlags |= NEED_RESOLUTION_MASK;
            }

            /**
             * Sets the relative start margin. Margin values should be positive.
             *
             * @param start the start margin size
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginStart
             */
            public void setMarginStart(int start)
            {
                startMargin = start;
                mMarginFlags |= NEED_RESOLUTION_MASK;
            }

            /**
             * Returns the start margin in pixels.
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginStart
             *
             * @return the start margin in pixels.
             */
            public int getMarginStart()
            {
                if (startMargin != DEFAULT_MARGIN_RELATIVE) return startMargin;
                if ((mMarginFlags & NEED_RESOLUTION_MASK) == NEED_RESOLUTION_MASK)
                {
                    doResolveMargins();
                }
                switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                {
                    //case View.LAYOUT_DIRECTION_RTL:
                    //return rightMargin;
                    //case View.LAYOUT_DIRECTION_LTR:
                    default:
                        return leftMargin;
                }
            }

            /**
             * Sets the relative end margin. Margin values should be positive.
             *
             * @param end the end margin size
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginEnd
             */
            public void setMarginEnd(int end)
            {
                endMargin = end;
                mMarginFlags |= NEED_RESOLUTION_MASK;
            }

            /**
             * Returns the end margin in pixels.
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginEnd
             *
             * @return the end margin in pixels.
             */
            public int getMarginEnd()
            {
                if (endMargin != DEFAULT_MARGIN_RELATIVE) return endMargin;
                if ((mMarginFlags & NEED_RESOLUTION_MASK) == NEED_RESOLUTION_MASK)
                {
                    doResolveMargins();
                }
                switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                {
                    //case View.LAYOUT_DIRECTION_RTL:
                    //return leftMargin;
                    //case View.LAYOUT_DIRECTION_LTR:
                    default:
                        return rightMargin;
                }
            }

            /**
             * Check if margins are relative.
             *
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginStart
             * @attr ref android.R.styleable#View_MarginLayout_layout_marginEnd
             *
             * @return true if either marginStart or marginEnd has been set.
             */
            public bool isMarginRelative()
            {
                return (startMargin != DEFAULT_MARGIN_RELATIVE || endMargin != DEFAULT_MARGIN_RELATIVE);
            }

            /**
             * Set the layout direction
             * @param layoutDirection the layout direction.
             *        Should be either {@link View#LAYOUT_DIRECTION_LTR}
             *                     or {@link View#LAYOUT_DIRECTION_RTL}.
             */
            public void setLayoutDirection(int layoutDirection)
            {
                //if (layoutDirection != View.LAYOUT_DIRECTION_LTR &&
                //layoutDirection != View.LAYOUT_DIRECTION_RTL) return;
                if (layoutDirection != (mMarginFlags & LAYOUT_DIRECTION_MASK))
                {
                    mMarginFlags &= ~LAYOUT_DIRECTION_MASK;
                    mMarginFlags |= (layoutDirection & LAYOUT_DIRECTION_MASK);
                    if (isMarginRelative())
                    {
                        mMarginFlags |= NEED_RESOLUTION_MASK;
                    }
                    else
                    {
                        mMarginFlags &= ~NEED_RESOLUTION_MASK;
                    }
                }
            }

            /**
             * Retuns the layout direction. Can be either {@link View#LAYOUT_DIRECTION_LTR} or
             * {@link View#LAYOUT_DIRECTION_RTL}.
             *
             * @return the layout direction.
             */
            public int getLayoutDirection()
            {
                return (mMarginFlags & LAYOUT_DIRECTION_MASK);
            }

            /**
             * This will be called by {@link android.view.View#requestLayout()}. Left and Right margins
             * may be overridden depending on layout direction.
             */
            public override void resolveLayoutDirection(int layoutDirection)
            {
                setLayoutDirection(layoutDirection);

                // No relative margin or pre JB-MR1 case or no need to resolve, just dont do anything
                // Will use the left and right margins if no relative margin is defined.
                if (!isMarginRelative() ||
                        (mMarginFlags & NEED_RESOLUTION_MASK) != NEED_RESOLUTION_MASK) return;

                // Proceed with resolution
                doResolveMargins();
            }

            private void doResolveMargins()
            {
                if ((mMarginFlags & RTL_COMPATIBILITY_MODE_MASK) == RTL_COMPATIBILITY_MODE_MASK)
                {
                    // if left or right margins are not defined and if we have some start or end margin
                    // defined then use those start and end margins.
                    if ((mMarginFlags & LEFT_MARGIN_UNDEFINED_MASK) == LEFT_MARGIN_UNDEFINED_MASK
                            && startMargin > DEFAULT_MARGIN_RELATIVE)
                    {
                        leftMargin = startMargin;
                    }
                    if ((mMarginFlags & RIGHT_MARGIN_UNDEFINED_MASK) == RIGHT_MARGIN_UNDEFINED_MASK
                            && endMargin > DEFAULT_MARGIN_RELATIVE)
                    {
                        rightMargin = endMargin;
                    }
                }
                else
                {
                    // We have some relative margins (either the start one or the end one or both). So use
                    // them and override what has been defined for left and right margins. If either start
                    // or end margin is not defined, just set it to default "0".
                    switch (mMarginFlags & LAYOUT_DIRECTION_MASK)
                    {
                        case View.LAYOUT_DIRECTION_RTL:
                            leftMargin = (endMargin > DEFAULT_MARGIN_RELATIVE) ?
                                    endMargin : DEFAULT_MARGIN_RESOLVED;
                            rightMargin = (startMargin > DEFAULT_MARGIN_RELATIVE) ?
                                    startMargin : DEFAULT_MARGIN_RESOLVED;
                            break;
                        case View.LAYOUT_DIRECTION_LTR:
                        default:
                            leftMargin = (startMargin > DEFAULT_MARGIN_RELATIVE) ?
                                    startMargin : DEFAULT_MARGIN_RESOLVED;
                            rightMargin = (endMargin > DEFAULT_MARGIN_RELATIVE) ?
                                    endMargin : DEFAULT_MARGIN_RESOLVED;
                            break;
                    }
                }
                mMarginFlags &= ~NEED_RESOLUTION_MASK;
            }

            /**
             * @hide
             */
            public bool isLayoutRtl()
            {
                //return ((mMarginFlags & LAYOUT_DIRECTION_MASK) == View.LAYOUT_DIRECTION_RTL);
                return false;
            }

            /**
             * @hide
             */
            public override void onDebugDraw(View view, SKCanvas canvas, SKPaint paint)
            {
                Insets oi = Insets.NONE; //isLayoutModeOptical(view.mParent) ? view.getOpticalInsets() : Insets.NONE;

                fillDifference(canvas,
                        view.getLeft() + oi.left,
                        view.getTop() + oi.top,
                        view.getRight() - oi.right,
                        view.getBottom() - oi.bottom,
                        leftMargin,
                        topMargin,
                        rightMargin,
                        bottomMargin,
                        paint);
            }
        }

        /**
         * Indicates whether or not this view's layout is right-to-left. This is resolved from
         * layout attribute and/or the inherited value from the parent
         *
         * @return true if the layout is right-to-left.
         *
         * @hide
         */
        public bool isLayoutRtl()
        {
            return false; // (getLayoutDirection() == LAYOUT_DIRECTION_RTL);
        }


        /**
         * Top position of this view relative to its parent.
         *
         * @return The top of this view, in pixels.
         */
        public int getTop()
        {
            return mTop;
        }

        /**
         * Returns true if the transform matrix is the identity matrix.
         * Recomputes the matrix if necessary.
         *
         * @return True if the transform matrix is the identity matrix, false otherwise.
         * @hide
         */
        public bool hasIdentityMatrix()
        {
            //return mRenderNode.hasIdentityMatrix();
            return false;
        }

        public class TransformationInfo
        {
            /**
             * The transform matrix for the View. This transform is calculated internally
             * based on the translation, rotation, and scale properties.
             *
             * Do *not* use this variable directly; instead call getMatrix(), which will
             * load the value from the View's RenderNode.
             */
            public readonly SKMatrix mMatrix = new SKMatrix();

            /**
             * The inverse transform matrix for the View. This transform is calculated
             * internally based on the translation, rotation, and scale properties.
             *
             * Do *not* use this variable directly; instead call getInverseMatrix(),
             * which will load the value from the View's RenderNode.
             */
            public SKMatrix mInverseMatrix;

            /**
             * The opacity of the View. This is a value from 0 to 1, where 0 means
             * completely transparent and 1 means completely opaque.
             */
            public float mAlpha = 1f;

            /**
             * The opacity of the view as manipulated by the Fade transition. This is a
             * property only used by transitions, which is composited with the other alpha
             * values to calculate the visual alpha value.
             */
            public float mTransitionAlpha = 1f;
        }

        /** @hide */
        public TransformationInfo mTransformationInfo;

        void ensureTransformationInfo()
        {
            if (mTransformationInfo == null)
            {
                mTransformationInfo = new TransformationInfo();
            }
        }

        /**
         * Utility method to retrieve the inverse of the current mMatrix property.
         * We cache the matrix to avoid recalculating it when transform properties
         * have not changed.
         *
         * @return The inverse of the current matrix of this view.
         * @hide
         */
        public SKMatrix getInverseMatrix()
        {
            ensureTransformationInfo();
            if (mTransformationInfo.mInverseMatrix == null)
            {
                mTransformationInfo.mInverseMatrix = new SKMatrix();
            }
            //mRenderNode.getInverseMatrix(matrix);
            return mTransformationInfo.mInverseMatrix.Invert();
        }


        /**
         * Sets the top position of this view relative to its parent. This method is meant to be called
         * by the layout system and should not generally be called otherwise, because the property
         * may be changed at any time by the layout.
         *
         * @param top The top of this view, in pixels.
         */
        public void setTop(int top)
        {
            if (top != mTop)
            {
                bool matrixIsIdentity = hasIdentityMatrix();
                if (matrixIsIdentity)
                {
                    if (mAttachInfo != null)
                    {
                        int minTop;
                        int yLoc;
                        if (top < mTop)
                        {
                            minTop = top;
                            yLoc = top - mTop;
                        }
                        else
                        {
                            minTop = mTop;
                            yLoc = 0;
                        }
                        invalidate(0, yLoc, mRight - mLeft, mBottom - minTop);
                    }
                }
                else
                {
                    // Double-invalidation is necessary to capture view's old and new areas
                    invalidate(true);
                }

                int width = mRight - mLeft;
                int oldHeight = mBottom - mTop;

                mTop = top;
                //mRenderNode.setTop(mTop);

                sizeChange(width, mBottom - mTop, width, oldHeight);

                if (!matrixIsIdentity)
                {
                    mPrivateFlags |= PFLAG_DRAWN; // force another invalidation with the new orientation
                    invalidate(true);
                }
                //mBackgroundSizeChanged = true;
                //mDefaultFocusHighlightSizeChanged = true;
                //if (mForegroundInfo != null)
                //{
                //    mForegroundInfo.mBoundsChanged = true;
                //}
                invalidateParentIfNeeded();
            }
        }

        /**
         * Used to indicate that the parent of this view should be invalidated. This functionality
         * is used to force the parent to rebuild its display list (when hardware-accelerated),
         * which is necessary when various parent-managed properties of the view change, such as
         * alpha, translationX/Y, scrollX/Y, scaleX/Y, and rotation/X/Y. This method will propagate
         * an invalidation event to the parent.
         *
         * @hide
         */
        protected void invalidateParentIfNeeded()
        {
            if (
                //isHardwareAccelerated() &&
                mParent is View
            )
            {
                ((View)mParent).invalidate(true);
            }
        }

        /**
         * Bottom position of this view relative to its parent.
         *
         * @return The bottom of this view, in pixels.
         */
        public int getBottom()
        {
            return mBottom;
        }

        /**
         * Sets the bottom position of this view relative to its parent. This method is meant to be
         * called by the layout system and should not generally be called otherwise, because the
         * property may be changed at any time by the layout.
         *
         * @param bottom The bottom of this view, in pixels.
         */
        public void setBottom(int bottom)
        {
            if (bottom != mBottom)
            {
                bool matrixIsIdentity = hasIdentityMatrix();
                if (matrixIsIdentity)
                {
                    if (mAttachInfo != null)
                    {
                        int maxBottom;
                        if (bottom < mBottom)
                        {
                            maxBottom = mBottom;
                        }
                        else
                        {
                            maxBottom = bottom;
                        }
                        invalidate(0, 0, mRight - mLeft, maxBottom - mTop);
                    }
                }
                else
                {
                    // Double-invalidation is necessary to capture view's old and new areas
                    invalidate(true);
                }

                int width = mRight - mLeft;
                int oldHeight = mBottom - mTop;

                mBottom = bottom;
                //mRenderNode.setBottom(mBottom);

                sizeChange(width, mBottom - mTop, width, oldHeight);

                if (!matrixIsIdentity)
                {
                    mPrivateFlags |= PFLAG_DRAWN; // force another invalidation with the new orientation
                    invalidate(true);
                }
                //mBackgroundSizeChanged = true;
                //mDefaultFocusHighlightSizeChanged = true;
                //if (mForegroundInfo != null)
                //{
                //    mForegroundInfo.mBoundsChanged = true;
                //}
                invalidateParentIfNeeded();
            }
        }

        /**
         * Left position of this view relative to its parent.
         *
         * @return The left edge of this view, in pixels.
         */
        public int getLeft()
        {
            return mLeft;
        }

        /**
         * Sets the left position of this view relative to its parent. This method is meant to be called
         * by the layout system and should not generally be called otherwise, because the property
         * may be changed at any time by the layout.
         *
         * @param left The left of this view, in pixels.
         */
        public void setLeft(int left)
        {
            if (left != mLeft)
            {
                bool matrixIsIdentity = hasIdentityMatrix();
                if (matrixIsIdentity)
                {
                    if (mAttachInfo != null)
                    {
                        int minLeft;
                        int xLoc;
                        if (left < mLeft)
                        {
                            minLeft = left;
                            xLoc = left - mLeft;
                        }
                        else
                        {
                            minLeft = mLeft;
                            xLoc = 0;
                        }
                        invalidate(xLoc, 0, mRight - minLeft, mBottom - mTop);
                    }
                }
                else
                {
                    // Double-invalidation is necessary to capture view's old and new areas
                    invalidate(true);
                }

                int oldWidth = mRight - mLeft;
                int height = mBottom - mTop;

                mLeft = left;
                //mRenderNode.setLeft(left);

                sizeChange(mRight - mLeft, height, oldWidth, height);

                if (!matrixIsIdentity)
                {
                    mPrivateFlags |= PFLAG_DRAWN; // force another invalidation with the new orientation
                    invalidate(true);
                }
                //mBackgroundSizeChanged = true;
                //mDefaultFocusHighlightSizeChanged = true;
                //if (mForegroundInfo != null)
                //{
                //    mForegroundInfo.mBoundsChanged = true;
                //}
                invalidateParentIfNeeded();
            }
        }

        /**
         * Right position of this view relative to its parent.
         *
         * @return The right edge of this view, in pixels.
         */
        public int getRight()
        {
            return mRight;
        }

        /**
         * Sets the right position of this view relative to its parent. This method is meant to be called
         * by the layout system and should not generally be called otherwise, because the property
         * may be changed at any time by the layout.
         *
         * @param right The right of this view, in pixels.
         */
        public void setRight(int right)
        {
            if (right != mRight)
            {
                bool matrixIsIdentity = hasIdentityMatrix();
                if (matrixIsIdentity)
                {
                    if (mAttachInfo != null)
                    {
                        int maxRight;
                        if (right < mRight)
                        {
                            maxRight = mRight;
                        }
                        else
                        {
                            maxRight = right;
                        }
                        invalidate(0, 0, maxRight - mLeft, mBottom - mTop);
                    }
                }
                else
                {
                    // Double-invalidation is necessary to capture view's old and new areas
                    invalidate(true);
                }

                int oldWidth = mRight - mLeft;
                int height = mBottom - mTop;

                mRight = right;
                //mRenderNode.setRight(mRight);

                sizeChange(mRight - mLeft, height, oldWidth, height);

                if (!matrixIsIdentity)
                {
                    mPrivateFlags |= PFLAG_DRAWN; // force another invalidation with the new orientation
                    invalidate(true);
                }
                invalidateParentIfNeeded();
            }
        }

        /**
         * Prior to N, some ViewGroups would not convert LayoutParams properly even though both extend
         * MarginLayoutParams. For instance, converting LinearLayout.LayoutParams to
         * RelativeLayout.LayoutParams would lose margin information. This is fixed on N but target API
         * check is implemented for backwards compatibility.
         *
         * {@hide}
         */
        protected const bool sPreserveMarginParamsInLayoutParamConversion = true; // always true




























        // GROUP


        /**
         * Returns a safe set of layout parameters based on the supplied layout params.
         * When a View is passed a View whose layout params do not pass the test of
         * {@link #checkLayoutParams(android.view.View.LayoutParams)}, this method
         * is invoked. This method should return a new set of layout params suitable for
         * this View, possibly by copying the appropriate attributes from the
         * specified set of layout params.
         *
         * @param p The layout parameters to convert into a suitable set of layout parameters
         *          for this View.
         *
         * @return an instance of {@link android.view.View.LayoutParams} or one
         *         of its descendants
         */
        protected virtual LayoutParams generateLayoutParams(LayoutParams p)
        {
            return p;
        }

        /**
         * Returns a set of default layout parameters. These parameters are requested
         * when the View passed to {@link #addView(View)} has no layout parameters
         * already set. If null is returned, an exception is thrown from addView.
         *
         * @return a set of default layout parameters or null
         */
        protected virtual LayoutParams generateDefaultLayoutParams()
        {
            return new LayoutParams(WRAP_CONTENT, WRAP_CONTENT);
        }

        public void updateViewLayout(View view, LayoutParams layout_params)
        {
            if (!checkLayoutParams(layout_params))
            {
                throw new Exception("Invalid LayoutParams supplied to " + this);
            }
            if (view.mParent != this)
            {
                throw new Exception("Given view not a child of " + this);
            }
            view.mLayoutParams = layout_params;
        }

        protected virtual bool checkLayoutParams(View.LayoutParams p)
        {
            return p != null;
        }

        /**
         * <p>Adds a child view. If no layout parameters are already set on the child, the
         * default parameters for this View are set on the child.</p>
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param child the child view to add
         *
         * @see #generateDefaultLayoutParams()
         */
        public void addView(View child)
        {
            addView(child, -1);
        }

        /**
         * Adds a child view. If no layout parameters are already set on the child, the
         * default parameters for this View are set on the child.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param child the child view to add
         * @param index the position at which to add the child
         *
         * @see #generateDefaultLayoutParams()
         */
        public void addView(View child, int index)
        {
            if (child == null)
            {
                throw new Exception("Cannot add a null child view to a View");
            }
            LayoutParams layout_params = child.mLayoutParams;
            if (layout_params == null)
            {
                layout_params = generateDefaultLayoutParams();
                if (layout_params == null)
                {
                    throw new Exception(
                            "generateDefaultLayoutParams() cannot return null  ");
                }
            }
            addView(child, index, layout_params);
        }

        /**
         * Adds a child view with this View's default layout parameters and the
         * specified width and height.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param child the child view to add
         */
        public void addView(View child, int width, int height)
        {
            LayoutParams layout_params = generateDefaultLayoutParams();
            layout_params.width = width;
            layout_params.height = height;
            addView(child, -1, layout_params);
        }

        /**
         * Adds a child view with the specified layout parameters.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param child the child view to add
         * @param params the layout parameters to set on the child
         */
        public void addView(View child, LayoutParams layout_params)
        {
            addView(child, -1, layout_params);
        }

        /**
         * Adds a child view with the specified layout parameters.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param child the child view to add
         * @param index the position at which to add the child or -1 to add last
         * @param params the layout parameters to set on the child
         */
        public void addView(View child, int index, LayoutParams layout_params)
        {
            if (child == null)
            {
                throw new Exception("Cannot add a null child view to a View");
            }

            // addViewInner() will call child.requestLayout() when setting the new LayoutParams
            // therefore, we call requestLayout() on ourselves before, so that the child's request
            // will be blocked at our level
            requestLayout();
            invalidate(true);
            addViewInner(child, index, layout_params, false);
        }

        // Child views of this View
        private View[] mChildren;
        // Number of valid children in the mChildren array, the rest should be null or not
        // considered as children
        private int mChildrenCount;

        internal const int ARRAY_INITIAL_CAPACITY = 12;
        internal const int ARRAY_CAPACITY_INCREMENT = 12;
        protected int mGroupFlags;

        /**
         * NOTE: If you change the flags below make sure to reflect the changes
         *       the DisplayList class
         */

        // When set, View invalidates only the child's rectangle
        // Set by default
        internal const int FLAG_CLIP_CHILDREN = 0x1;

        // When set, View excludes the padding area from the invalidate rectangle
        // Set by default
        internal const int FLAG_CLIP_TO_PADDING = 0x2;

        // When set, dispatchDraw() will invoke invalidate(); this is set by drawChild() when
        // a child needs to be invalidated and FLAG_OPTIMIZE_INVALIDATE is set
        internal const int FLAG_INVALIDATE_REQUIRED = 0x4;

        // If set, this View has padding; if unset there is no padding and we don't need
        // to clip it, even if FLAG_CLIP_TO_PADDING is set
        internal const int FLAG_PADDING_NOT_NULL = 0x20;

        // When set, the next call to drawChild() will clear mChildTransformation's matrix
        internal const int FLAG_CLEAR_TRANSFORMATION = 0x100;

        /**
         * When set, the drawing method will call {@link #getChildDrawingOrder(int, int)}
         * to get the index of the child to draw for that iteration.
         *
         * @hide
         */
        internal const int FLAG_USE_CHILD_DRAWING_ORDER = 0x400;

        /** @deprecated functionality removed */
        internal const int FLAG_ALWAYS_DRAWN_WITH_CACHE = 0x4000;

        /**
         * When set, this View will split MotionEvents to multiple child Views when appropriate.
         */
        internal const int FLAG_SPLIT_MOTION_EVENTS = 0x200000;

        /**
         * We clip to padding when FLAG_CLIP_TO_PADDING and FLAG_PADDING_NOT_NULL
         * are set at the same time.
         */
        protected const int CLIP_TO_PADDING_MASK = FLAG_CLIP_TO_PADDING | FLAG_PADDING_NOT_NULL;



        /**
         * Indicates which types of drawing caches are to be kept in memory.
         * This field should be made private, so it is hidden from the SDK.
         * {@hide}
         */
        protected int mPersistentDrawingCache;

        /**
         * Used to indicate that no drawing cache should be kept in memory.
         *
         * @deprecated The view drawing cache was largely made obsolete with the introduction of
         * hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
         * layers are largely unnecessary and can easily result in a net loss in performance due to the
         * cost of creating and updating the layer. In the rare cases where caching layers are useful,
         * such as for alpha animations, {@link #setLayerType(int, Paint)} handles this with hardware
         * rendering. For software-rendered snapshots of a small part of the View hierarchy or
         * individual Views it is recommended to create a {@link Canvas} from either a {@link Bitmap} or
         * {@link android.graphics.Picture} and call {@link #draw(Canvas)} on the View. However these
         * software-rendered usages are discouraged and have compatibility issues with hardware-only
         * rendering features such as {@link android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE}
         * bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
         * reports or unit testing the {@link PixelCopy} API is recommended.
         */
        public const int PERSISTENT_NO_CACHE = 0x0;

        /**
         * Used to indicate that the animation drawing cache should be kept in memory.
         *
         * @deprecated The view drawing cache was largely made obsolete with the introduction of
         * hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
         * layers are largely unnecessary and can easily result in a net loss in performance due to the
         * cost of creating and updating the layer. In the rare cases where caching layers are useful,
         * such as for alpha animations, {@link #setLayerType(int, Paint)} handles this with hardware
         * rendering. For software-rendered snapshots of a small part of the View hierarchy or
         * individual Views it is recommended to create a {@link Canvas} from either a {@link Bitmap} or
         * {@link android.graphics.Picture} and call {@link #draw(Canvas)} on the View. However these
         * software-rendered usages are discouraged and have compatibility issues with hardware-only
         * rendering features such as {@link android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE}
         * bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
         * reports or unit testing the {@link PixelCopy} API is recommended.
         */
        public const int PERSISTENT_ANIMATION_CACHE = 0x1;

        /**
         * Used to indicate that the scrolling drawing cache should be kept in memory.
         *
         * @deprecated The view drawing cache was largely made obsolete with the introduction of
         * hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
         * layers are largely unnecessary and can easily result in a net loss in performance due to the
         * cost of creating and updating the layer. In the rare cases where caching layers are useful,
         * such as for alpha animations, {@link #setLayerType(int, Paint)} handles this with hardware
         * rendering. For software-rendered snapshots of a small part of the View hierarchy or
         * individual Views it is recommended to create a {@link Canvas} from either a {@link Bitmap} or
         * {@link android.graphics.Picture} and call {@link #draw(Canvas)} on the View. However these
         * software-rendered usages are discouraged and have compatibility issues with hardware-only
         * rendering features such as {@link android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE}
         * bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
         * reports or unit testing the {@link PixelCopy} API is recommended.
         */
        public const int PERSISTENT_SCROLLING_CACHE = 0x2;

        /**
         * Used to indicate that all drawing caches should be kept in memory.
         *
         * @deprecated The view drawing cache was largely made obsolete with the introduction of
         * hardware-accelerated rendering in API 11. With hardware-acceleration, intermediate cache
         * layers are largely unnecessary and can easily result in a net loss in performance due to the
         * cost of creating and updating the layer. In the rare cases where caching layers are useful,
         * such as for alpha animations, {@link #setLayerType(int, Paint)} handles this with hardware
         * rendering. For software-rendered snapshots of a small part of the View hierarchy or
         * individual Views it is recommended to create a {@link Canvas} from either a {@link Bitmap} or
         * {@link android.graphics.Picture} and call {@link #draw(Canvas)} on the View. However these
         * software-rendered usages are discouraged and have compatibility issues with hardware-only
         * rendering features such as {@link android.graphics.Bitmap.Config#HARDWARE Config.HARDWARE}
         * bitmaps, real-time shadows, and outline clipping. For screenshots of the UI for feedback
         * reports or unit testing the {@link PixelCopy} API is recommended.
         */
        public const int PERSISTENT_ALL_CACHES = 0x3;


        private void initView()
        {
            // View doesn't draw by default
            if (!isShowingLayoutBounds())
            {
                setFlags(WILL_NOT_DRAW, DRAW_MASK);
            }
            mGroupFlags |= FLAG_CLIP_CHILDREN;
            mGroupFlags |= FLAG_CLIP_TO_PADDING;
            //mGroupFlags |= FLAG_ANIMATION_DONE;
            //mGroupFlags |= FLAG_ANIMATION_CACHE;
            mGroupFlags |= FLAG_ALWAYS_DRAWN_WITH_CACHE;

            mGroupFlags |= FLAG_SPLIT_MOTION_EVENTS;

            mChildren = new View[ARRAY_INITIAL_CAPACITY];
            mChildrenCount = 0;

            mPersistentDrawingCache = PERSISTENT_SCROLLING_CACHE;
        }

        /**
         * If this view doesn't do any drawing on its own, set this flag to
         * allow further optimizations. By default, this flag is not set on
         * View, but could be set on some View subclasses such as ViewGroup.
         *
         * Typically, if you override {@link #onDraw(android.graphics.Canvas)}
         * you should clear this flag.
         *
         * @param willNotDraw whether or not this View draw on its own
         */
        public void setWillNotDraw(bool willNotDraw)
        {
            setFlags(willNotDraw ? WILL_NOT_DRAW : 0, DRAW_MASK);
        }

        /**
         * If this view doesn't do any drawing on its own, set this flag to
         * false to allow further optimizations. By default, this flag is
         * set on View.
         *
         * Typically, if you override {@link #onDraw(android.graphics.Canvas)}
         * you should clear this flag.
         *
         * @param willDraw whether or not this View draw on its own
         */
        public void setWillDraw(bool willDraw)
        {
            setFlags(willDraw ? 0 : WILL_NOT_DRAW, DRAW_MASK);
        }

        /**
         * Returns whether or not this View draws on its own.
         *
         * @return true if this view has nothing to draw, false otherwise
         */
        public bool willNotDraw()
        {
            return (mViewFlags & DRAW_MASK) == WILL_NOT_DRAW;
        }

        /**
         * Returns whether or not this View draws on its own.
         *
         * @return false if this view has nothing to draw, true otherwise
         */
        public bool willDraw()
        {
            return (mViewFlags & DRAW_MASK) != WILL_NOT_DRAW;
        }

        // For debugging only.  You can see these in hierarchyviewer.
        private long mLastTouchDownTime;
        private int mLastTouchDownIndex = -1;
        private float mLastTouchDownX;
        private float mLastTouchDownY;
        internal bool mRecreateDisplayList;

        private void addInArray(View child, int index)
        {
            View[] children = mChildren;
            int count = mChildrenCount;
            int size = children.Length;
            if (index == count)
            {
                if (size == count)
                {
                    mChildren = new View[size + ARRAY_CAPACITY_INCREMENT];
                    Array.ConstrainedCopy(children, 0, mChildren, 0, size);
                    children = mChildren;
                }
                children[mChildrenCount++] = child;
            }
            else if (index < count)
            {
                if (size == count)
                {
                    mChildren = new View[size + ARRAY_CAPACITY_INCREMENT];
                    Array.ConstrainedCopy(children, 0, mChildren, 0, index);
                    Array.ConstrainedCopy(children, index, mChildren, index + 1, count - index);
                    children = mChildren;
                }
                else
                {
                    Array.ConstrainedCopy(children, index, children, index + 1, count - index);
                }
                children[index] = child;
                mChildrenCount++;
                if (mLastTouchDownIndex >= index)
                {
                    mLastTouchDownIndex++;
                }
            }
            else
            {
                throw new Exception("index=" + index + " count=" + count);
            }
        }

        // This method also sets the child's Parent to null
        private void removeFromArray(int index)
        {
            View[] children = mChildren;
            //if (!(mTransitioningViews != null && mTransitioningViews.contains(children[index])))
            //{
            //children[index].Parent = null;
            //}
            int count = mChildrenCount;
            if (index == count - 1)
            {
                children[--mChildrenCount] = null;
            }
            else if (index >= 0 && index < count)
            {
                Array.ConstrainedCopy(children, index + 1, children, index, count - index - 1);
                children[--mChildrenCount] = null;
            }
            else
            {
                throw new Exception();
            }
            if (mLastTouchDownIndex == index)
            {
                mLastTouchDownTime = 0;
                mLastTouchDownIndex = -1;
            }
            else if (mLastTouchDownIndex > index)
            {
                mLastTouchDownIndex--;
            }
        }

        // This method also sets the children's Parent to null
        private void removeFromArray(int start, int count)
        {
            View[] children = mChildren;
            int childrenCount = mChildrenCount;

            start = Math.Max(0, start);
            int end = Math.Min(childrenCount, start + count);

            if (start == end)
            {
                return;
            }

            if (end == childrenCount)
            {
                for (int i = start; i < end; i++)
                {
                    children[i].mParent = null;
                    children[i] = null;
                }
            }
            else
            {
                for (int i = start; i < end; i++)
                {
                    children[i].mParent = null;
                }

                // Since we're looping above, we might as well do the copy, but is arraycopy()
                // faster than the extra 2 bounds checks we would do in the loop?
                Array.ConstrainedCopy(children, end, children, start, childrenCount - end);

                for (int i = childrenCount - (end - start); i < childrenCount; i++)
                {
                    children[i] = null;
                }
            }

            mChildrenCount -= (end - start);
        }

        /**
         * {@inheritDoc}
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         */
        public void removeView(View view)
        {
            if (removeViewInternal(view))
            {
                requestLayout();
                invalidate(true);
            }
        }

        /**
         * Removes a view during layout. This is useful if in your onLayout() method,
         * you need to remove more views.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param view the view to remove from the group
         */
        public void removeViewInLayout(View view)
        {
            removeViewInternal(view);
        }

        /**
         * Removes a range of views during layout. This is useful if in your onLayout() method,
         * you need to remove more views.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param start the index of the first view to remove from the group
         * @param count the number of views to remove from the group
         */
        public void removeViewsInLayout(int start, int count)
        {
            removeViewsInternal(start, count);
        }

        /**
         * Removes the view at the specified position in the group.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param index the position in the group of the view to remove
         */
        public void removeViewAt(int index)
        {
            removeViewInternal(index, getChildAt(index));
            requestLayout();
            invalidate(true);
        }

        /**
         * Removes the specified range of views from the group.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         *
         * @param start the first position in the group of the range of views to remove
         * @param count the number of views to remove
         */
        public void removeViews(int start, int count)
        {
            removeViewsInternal(start, count);
            requestLayout();
            invalidate(true);
        }

        private bool removeViewInternal(View view)
        {
            int index = indexOfChild(view);
            if (index >= 0)
            {
                removeViewInternal(index, view);
                return true;
            }
            return false;
        }

        private void removeViewInternal(int index, View view)
        {
            //if (mTransition != null)
            //{
            //    mTransition.removeChild(this, view);
            //}

            bool clearChildFocus = false;
            if (view == mFocused)
            {
                view.unFocus(null);
                clearChildFocus = true;
            }
            if (view == mFocusedInCluster)
            {
                clearFocusedInCluster(view);
            }

            //view.clearAccessibilityFocus();

            //cancelTouchTarget(view);
            //cancelHoverTarget(view);

            //if (view.getAnimation() != null ||
            //        (mTransitioningViews != null && mTransitioningViews.contains(view)))
            //{
            //    addDisappearingView(view);
            //}
            //else if (view.mAttachInfo != null)
            //{
            //    view.dispatchDetachedFromWindow();
            //}

            //if (view.hasTransientState())
            //{
            //    childHasTransientStateChanged(view, false);
            //}

            //needGlobalAttributesUpdate(false);

            removeFromArray(index);

            //if (view.hasUnhandledKeyListener())
            //{
            //    decrementChildUnhandledKeyListeners();
            //}

            if (view == mDefaultFocus)
            {
                clearDefaultFocus(view);
            }
            if (clearChildFocus)
            {
                this.clearChildFocus(view);
                //if (!rootViewRequestFocus())
                //{
                //    notifyGlobalFocusCleared(this);
                //}
            }

            dispatchViewRemoved(view);

            //if (view.getVisibility() != View.GONE)
            //{
            //    notifySubtreeAccessibilityStateChangedIfNeeded();
            //}

            //int transientCount = mTransientIndices == null ? 0 : mTransientIndices.size();
            //for (int i = 0; i < transientCount; ++i)
            //{
            //    int oldIndex = mTransientIndices.get(i);
            //    if (index < oldIndex)
            //    {
            //        mTransientIndices.set(i, oldIndex - 1);
            //    }
            //}

            //if (mCurrentDragStartEvent != null)
            //{
            //    mChildrenInterestedInDrag.remove(view);
            //}
        }

        public void clearChildFocus(View child)
        {
            mFocused = null;
            if (mParent != null)
            {
                mParent.clearChildFocus(this);
            }
        }

        private void removeViewsInternal(int start, int count)
        {
            int end = start + count;

            if (start < 0 || count < 0 || end > mChildrenCount)
            {
                throw new Exception();
            }

            View focused = mFocused;
            bool detach = mAttachInfo != null;
            bool clearChildFocus = false;
            View clearDefaultFocus = null;

            View[] children = mChildren;

            for (int i = start; i < end; i++)
            {
                View view = children[i];

                //if (mTransition != null)
                //{
                //    mTransition.removeChild(this, view);
                //}

                if (view == focused)
                {
                    view.unFocus(null);
                    clearChildFocus = true;
                }
                if (view == mDefaultFocus)
                {
                    clearDefaultFocus = view;
                }
                if (view == mFocusedInCluster)
                {
                    clearFocusedInCluster(view);
                }

                //view.clearAccessibilityFocus();

                //cancelTouchTarget(view);
                //cancelHoverTarget(view);

                //if (view.getAnimation() != null ||
                //    (mTransitioningViews != null && mTransitioningViews.contains(view)))
                //{
                //    addDisappearingView(view);
                //}
                //else if (detach)
                //{
                //    view.dispatchDetachedFromWindow();
                //}

                //if (view.hasTransientState())
                //{
                //    childHasTransientStateChanged(view, false);
                //}

                //needGlobalAttributesUpdate(false);

                dispatchViewRemoved(view);
            }

            removeFromArray(start, count);

            if (clearDefaultFocus != null)
            {
                this.clearDefaultFocus(clearDefaultFocus);
            }
            if (clearChildFocus)
            {
                this.clearChildFocus(focused);
                //if (!rootViewRequestFocus())
                //{
                //notifyGlobalFocusCleared(focused);
                //}
            }
        }

        /**
         * Call this method to remove all child views from the
         * View.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         */
        public void removeAllViews()
        {
            removeAllViewsInLayout();
            requestLayout();
            invalidate(true);
        }

        /**
         * Called by a View subclass to remove child views from itself,
         * when it must first know its size on screen before it can calculate how many
         * child views it will render. An example is a Gallery or a ListView, which
         * may "have" 50 children, but actually only render the number of children
         * that can currently fit inside the object on screen. Do not call
         * this method unless you are extending View and understand the
         * view measuring and layout pipeline.
         *
         * <p><strong>Note:</strong> do not invoke this method from
         * {@link #draw(android.graphics.Canvas)}, {@link #onDraw(android.graphics.Canvas)},
         * {@link #dispatchDraw(android.graphics.Canvas)} or any related method.</p>
         */
        public void removeAllViewsInLayout()
        {
            int count = mChildrenCount;
            if (count <= 0)
            {
                return;
            }

            View[] children = mChildren;
            mChildrenCount = 0;

            View focused = mFocused;
            bool detach = mAttachInfo != null;
            bool clearChildFocus = false;

            //needGlobalAttributesUpdate(false);

            for (int i = count - 1; i >= 0; i--)
            {
                View view = children[i];

                //if (mTransition != null)
                //{
                //    mTransition.removeChild(this, view);
                //}

                if (view == focused)
                {
                    view.unFocus(null);
                    clearChildFocus = true;
                }

                //view.clearAccessibilityFocus();

                //cancelTouchTarget(view);
                //cancelHoverTarget(view);

                //if (view.getAnimation() != null ||
                //        (mTransitioningViews != null && mTransitioningViews.contains(view)))
                //{
                //    addDisappearingView(view);
                //}
                //else if (detach)
                //{
                //    view.dispatchDetachedFromWindow();
                //}

                //if (view.hasTransientState())
                //{
                //    childHasTransientStateChanged(view, false);
                //}

                dispatchViewRemoved(view);

                view.mParent = null;
                children[i] = null;
            }

            if (mDefaultFocus != null)
            {
                clearDefaultFocus(mDefaultFocus);
            }
            if (mFocusedInCluster != null)
            {
                clearFocusedInCluster(mFocusedInCluster);
            }
            if (clearChildFocus)
            {
                this.clearChildFocus(focused);
                //if (!rootViewRequestFocus())
                //{
                //    notifyGlobalFocusCleared(focused);
                //}
            }
        }


        void dispatchViewAdded(View child)
        {
            onViewAdded(child);
            //if (mOnHierarchyChangeListener != null)
            //{
            //mOnHierarchyChangeListener.onChildViewAdded(this, child);
            //}
        }

        /**
         * Called when a new child is added to this View. Overrides should always
         * call super.onViewAdded.
         *
         * @param child the added child view
         */
        public void onViewAdded(View child)
        {
        }

        void dispatchViewRemoved(View child)
        {
            onViewRemoved(child);
            //if (mOnHierarchyChangeListener != null)
            //{
            //mOnHierarchyChangeListener.onChildViewRemoved(this, child);
            //}
        }

        /**
         * Called when a child view is removed from this View. Overrides should always
         * call super.onViewRemoved.
         *
         * @param child the removed child view
         */
        public void onViewRemoved(View child)
        {
        }

        /**
         * <p>Indicates that this view gets its drawable states from its direct parent
         * and ignores its original internal states.</p>
         *
         * @hide
         */
        internal const int DUPLICATE_PARENT_STATE = 0x00400000;

        /**
         * When set, this ViewGroup will not dispatch onAttachedToWindow calls
         * to children when adding new views. This is used to prevent multiple
         * onAttached calls when a ViewGroup adds children in its own onAttached method.
         */
        private const int FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW = 0x400000;

        /**
         * When set, this group will go through its list of children to notify them of
         * any drawable state change.
         */
        internal const int FLAG_NOTIFY_CHILDREN_ON_DRAWABLE_STATE_CHANGE = 0x10000;

        private void addViewInner(View child, int index, LayoutParams layout_params,
            bool preventRequestLayout)
        {

            //if (mTransition != null)
            //{
            //    // Don't prevent other add transitions from completing, but cancel remove
            //    // transitions to let them complete the process before we add to the container
            //    mTransition.cancel(LayoutTransition.DISAPPEARING);
            //}

            if (child.mParent != null)
            {
                throw new Exception("The specified child already has a parent. " +
                        "You must call removeView() on the child's parent first.");
            }

            //if (mTransition != null)
            //{
            //    mTransition.addChild(this, child);
            //}

            if (!checkLayoutParams(layout_params))
            {
                layout_params = generateLayoutParams(layout_params);
            }

            if (preventRequestLayout)
            {
                child.mLayoutParams = layout_params;
            }
            else
            {
                child.mLayoutParams = layout_params;
            }

            if (index < 0)
            {
                index = mChildrenCount;
            }

            addInArray(child, index);

            // tell our children
            if (preventRequestLayout)
            {
                child.assignParent(this);
            }
            else
            {
                child.mParent = this;
            }
            //if (child.hasUnhandledKeyListener())
            //{
            //incrementChildUnhandledKeyListeners();
            //}

            bool childHasFocus = child.hasFocus();
            if (childHasFocus)
            {
                requestChildFocus(child, child.findFocus());
            }

            AttachInfo ai = mAttachInfo;
            if (ai != null && (mGroupFlags & FLAG_PREVENT_DISPATCH_ATTACHED_TO_WINDOW) == 0)
            {
                //bool lastKeepOn = ai.mKeepScreenOn;
                //ai.mKeepScreenOn = false;
                child.dispatchAttachedToWindow(mAttachInfo, (mViewFlags & VISIBILITY_MASK));
                //if (ai.mKeepScreenOn)
                //{
                    //needGlobalAttributesUpdate(true);
                //}
                //ai.mKeepScreenOn = lastKeepOn;
            }

            if (child.isLayoutDirectionInherited())
            {
                child.resetRtlProperties();
            }

            dispatchViewAdded(child);

            if ((child.mViewFlags & DUPLICATE_PARENT_STATE) == DUPLICATE_PARENT_STATE)
            {
                mGroupFlags |= FLAG_NOTIFY_CHILDREN_ON_DRAWABLE_STATE_CHANGE;
            }

            //if (child.hasTransientState())
            //{
            //childHasTransientStateChanged(child, true);
            //}

            //if (child.getVisibility() != View.GONE)
            //{
            //    notifySubtreeAccessibilityStateChangedIfNeeded();
            //}

            //if (mTransientIndices != null)
            //{
            //    int transientCount = mTransientIndices.size();
            //    for (int i = 0; i < transientCount; ++i)
            //    {
            //        int oldIndex = mTransientIndices.get(i);
            //        if (index <= oldIndex)
            //        {
            //            mTransientIndices.set(i, oldIndex + 1);
            //        }
            //    }
            //}

            //if (mCurrentDragStartEvent != null && child.getVisibility() == VISIBLE)
            //{
            //    notifyChildOfDragStart(child);
            //}

            if (child.hasDefaultFocus())
            {
                // When adding a child that contains default focus, either during inflation or while
                // manually assembling the hierarchy, update the ancestor default-focus chain.
                setDefaultFocus(child);
            }

            //touchAccessibilityNodeProviderIfNeeded(child);
        }



        /**
         * Returns the position in the group of the specified child view.
         *
         * @param child the view for which to get the position
         * @return a positive integer representing the position of the view in the
         *         group, or -1 if the view does not exist in the group
         */
        public int indexOfChild(View child)
        {
            int count = mChildrenCount;
            View[] children = mChildren;
            for (int i = 0; i < count; i++)
            {
                if (children[i] == child)
                {
                    return i;
                }
            }
            return -1;
        }

        /**
         * Returns the number of children in the group.
         *
         * @return a positive integer representing the number of children in
         *         the group
         */
        public int getChildCount()
        {
            return mChildrenCount;
        }

        /**
         * Returns the view at the specified position in the group.
         *
         * @param index the position at which to get the view from
         * @return the view at the specified position or null if the position
         *         does not exist within the group
         */
        public View getChildAt(int index)
        {
            if (index < 0 || index >= mChildrenCount)
            {
                return null;
            }
            return mChildren[index];
        }

        /**
         * Returns the top padding of this view.
         *
         * @return the top padding in pixels
         */
        public int getPaddingTop()
        {
            return mPaddingTop;
        }

        /**
         * Returns the bottom padding of this view. If there are inset and enabled
         * scrollbars, this value may include the space required to display the
         * scrollbars as well.
         *
         * @return the bottom padding in pixels
         */
        public int getPaddingBottom()
        {
            return mPaddingBottom;
        }

        /**
         * Returns the left padding of this view. If there are inset and enabled
         * scrollbars, this value may include the space required to display the
         * scrollbars as well.
         *
         * @return the left padding in pixels
         */
        public int getPaddingLeft()
        {
            if (!isPaddingResolved())
            {
                resolvePadding();
            }
            return mPaddingLeft;
        }

        /**
         * Returns the start padding of this view depending on its resolved layout direction.
         * If there are inset and enabled scrollbars, this value may include the space
         * required to display the scrollbars as well.
         *
         * @return the start padding in pixels
         */
        public int getPaddingStart()
        {
            if (!isPaddingResolved())
            {
                resolvePadding();
            }
            return (getLayoutDirection() == LAYOUT_DIRECTION_RTL) ?
                    mPaddingRight : mPaddingLeft;
        }

        /**
         * Returns the right padding of this view. If there are inset and enabled
         * scrollbars, this value may include the space required to display the
         * scrollbars as well.
         *
         * @return the right padding in pixels
         */
        public int getPaddingRight()
        {
            if (!isPaddingResolved())
            {
                resolvePadding();
            }
            return mPaddingRight;
        }

        /**
         * Returns the end padding of this view depending on its resolved layout direction.
         * If there are inset and enabled scrollbars, this value may include the space
         * required to display the scrollbars as well.
         *
         * @return the end padding in pixels
         */
        public int getPaddingEnd()
        {
            if (!isPaddingResolved())
            {
                resolvePadding();
            }
            return (getLayoutDirection() == LAYOUT_DIRECTION_RTL) ?
                    mPaddingLeft : mPaddingRight;
        }

        Insets computeOpticalInsets()
        {
            return Insets.NONE; // (mBackground == null) ? Insets.NONE : mBackground.getOpticalInsets();
        }

        /**
         * @hide
         */
        public Insets getOpticalInsets()
        {
            if (mLayoutInsets == null)
            {
                mLayoutInsets = computeOpticalInsets();
            }
            return mLayoutInsets;
        }

        /**
         * Set this view's optical insets.
         *
         * <p>This method should be treated similarly to setMeasuredDimension and not as a general
         * property. Views that compute their own optical insets should call it as part of measurement.
         * This method does not request layout. If you are setting optical insets outside of
         * measure/layout itself you will want to call requestLayout() yourself.
         * </p>
         * @hide
         */
        public void setOpticalInsets(Insets insets)
        {
            mLayoutInsets = insets;
        }


        private static void fillRect(SKCanvas canvas, SKPaint paint, int x1, int y1, int x2, int y2)
        {
            if (x1 != x2 && y1 != y2)
            {
                if (x1 > x2)
                {
                    int tmp = x1; x1 = x2; x2 = tmp;
                }
                if (y1 > y2)
                {
                    int tmp = y1; y1 = y2; y2 = tmp;
                }
                canvas.DrawRectCoords(x1, y1, x2, y2, paint);
            }
        }

        private static int sign(int x)
        {
            return (x >= 0) ? 1 : -1;
        }

        private static void drawCorner(SKCanvas c, SKPaint paint, int x1, int y1, int dx, int dy, int lw)
        {
            fillRect(c, paint, x1, y1, x1 + dx, y1 + lw * sign(dy));
            fillRect(c, paint, x1, y1, x1 + lw * sign(dx), y1 + dy);
        }

        private static void drawRectCorners(SKCanvas canvas, int x1, int y1, int x2, int y2, SKPaint paint,
                int lineLength, int lineWidth)
        {
            drawCorner(canvas, paint, x1, y1, lineLength, lineLength, lineWidth);
            drawCorner(canvas, paint, x1, y2, lineLength, -lineLength, lineWidth);
            drawCorner(canvas, paint, x2, y1, -lineLength, lineLength, lineWidth);
            drawCorner(canvas, paint, x2, y2, -lineLength, -lineLength, lineWidth);
        }

        private static void fillDifference(SKCanvas canvas,
                int x2, int y2, int x3, int y3,
                int dx1, int dy1, int dx2, int dy2, SKPaint paint)
        {
            int x1 = x2 - dx1;
            int y1 = y2 - dy1;

            int x4 = x3 + dx2;
            int y4 = y3 + dy2;

            fillRect(canvas, paint, x1, y1, x4, y2);
            fillRect(canvas, paint, x1, y2, x2, y3);
            fillRect(canvas, paint, x3, y2, x4, y3);
            fillRect(canvas, paint, x1, y3, x4, y4);
        }

        /**
         * @hide
         */
        protected void onDebugDrawMargins(SKCanvas canvas, SKPaint paint)
        {
            for (int i = 0; i < getChildCount(); i++)
            {
                View c = getChildAt(i);
                c.mLayoutParams.onDebugDraw(c, canvas, paint);
            }
        }

        private static void drawRect(SKCanvas canvas, SKPaint paint, float x1, float y1, float x2, float y2)
        {
            if (sDebugLines == null)
            {
                // TODO: This won't work with multiple UI threads in a single process
                sDebugLines = new float[16];
            }

            sDebugLines[0] = x1;
            sDebugLines[1] = y1;
            sDebugLines[2] = x2;
            sDebugLines[3] = y1;

            sDebugLines[4] = x2;
            sDebugLines[5] = y1;
            sDebugLines[6] = x2;
            sDebugLines[7] = y2;

            sDebugLines[8] = x2;
            sDebugLines[9] = y2;
            sDebugLines[10] = x1;
            sDebugLines[11] = y2;

            sDebugLines[12] = x1;
            sDebugLines[13] = y2;
            sDebugLines[14] = x1;
            sDebugLines[15] = y1;

            canvas.DrawLines(sDebugLines, paint);
        }

        /**
         * @hide
         */
        protected void onDebugDraw(SKCanvas canvas)
        {
            SKPaint paint = getDebugPaint();

            // Draw optical bounds
            {
                paint.ColorF = SKColors.Red;
                paint.Style = SKPaintStyle.Stroke;

                for (int i = 0; i < getChildCount(); i++)
                {
                    View c = getChildAt(i);
                    if (c.getVisibility() != View.GONE)
                    {
                        Insets insets = c.getOpticalInsets();

                        drawRect(canvas, paint,
                                c.getLeft() + insets.left,
                                c.getTop() + insets.top,
                                (c.getRight() - insets.right - 1),
                                (c.getBottom() - insets.bottom - 1));
                    }
                }
            }

            // Draw margins
            {
                paint.SetColor(63, 255, 0, 255);
                paint.Style = SKPaintStyle.Fill;

                onDebugDrawMargins(canvas, paint);
            }

            // Draw clip bounds
            {
                paint.SetColor(DEBUG_CORNERS_COLOR);
                paint.Style = SKPaintStyle.Fill;

                int lineLength = dipsToPixels(DEBUG_CORNERS_SIZE_DIP);
                int lineWidth = dipsToPixels(1);
                for (int i = 0; i < getChildCount(); i++)
                {
                    View c = getChildAt(i);
                    if (c.getVisibility() != View.GONE)
                    {
                        drawRectCorners(canvas, c.getLeft(), c.getTop(), c.getRight(), c.getBottom(),
                                paint, lineLength, lineWidth);
                    }
                }
            }
        }

        /**
         * <p>Return the time at which the drawing of the view hierarchy started.</p>
         *
         * @return the drawing start time in milliseconds
         */
        public long getDrawingTime()
        {
            return mAttachInfo != null ? mAttachInfo.mDrawingTime : 0;
        }

        /**
         * <p>Indicates whether this view is attached to a hardware accelerated
         * window or not.</p>
         *
         * <p>Even if this method returns true, it does not mean that every call
         * to {@link #draw(android.graphics.Canvas)} will be made with an hardware
         * accelerated {@link android.graphics.Canvas}. For instance, if this view
         * is drawn onto an offscreen {@link android.graphics.Bitmap} and its
         * window is hardware accelerated,
         * {@link android.graphics.Canvas#isHardwareAccelerated()} will likely
         * return false, and this method will return true.</p>
         *
         * @return True if the view is attached to a window and the window is
         *         hardware accelerated; false in any other case.
         */
        public bool isHardwareAccelerated()
        {
            return mAttachInfo != null && mAttachInfo.mHardwareAccelerated;
        }


        /**
         * The visual x position of this view, in pixels. This is equivalent to the
         * {@link #setTranslationX(float) translationX} property plus the current
         * {@link #getLeft() left} property.
         *
         * @return The visual x position of this view, in pixels.
         */
        public float getX()
        {
            return mLeft + getTranslationX();
        }


        /**
         * Sets the visual x position of this view, in pixels. This is equivalent to setting the
         * {@link #setTranslationX(float) translationX} property to be the difference between
         * the x value passed in and the current {@link #getLeft() left} property.
         *
         * @param x The visual x position of this view, in pixels.
         */
        public void setX(float x)
        {
            setTranslationX(x - mLeft);
        }

        /**
         * The visual y position of this view, in pixels. This is equivalent to the
         * {@link #setTranslationY(float) translationY} property plus the current
         * {@link #getTop() top} property.
         *
         * @return The visual y position of this view, in pixels.
         */
        public float getY()
        {
            return mTop + getTranslationY();
        }

        /**
         * Sets the visual y position of this view, in pixels. This is equivalent to setting the
         * {@link #setTranslationY(float) translationY} property to be the difference between
         * the y value passed in and the current {@link #getTop() top} property.
         *
         * @param y The visual y position of this view, in pixels.
         */
        public void setY(float y)
        {
            setTranslationY(y - mTop);
        }

        /**
         * The visual z position of this view, in pixels. This is equivalent to the
         * {@link #setTranslationZ(float) translationZ} property plus the current
         * {@link #getElevation() elevation} property.
         *
         * @return The visual z position of this view, in pixels.
         */
        public float getZ()
        {
            return getElevation() + getTranslationZ();
        }

        /**
         * Sets the visual z position of this view, in pixels. This is equivalent to setting the
         * {@link #setTranslationZ(float) translationZ} property to be the difference between
         * the z value passed in and the current {@link #getElevation() elevation} property.
         *
         * @param z The visual z position of this view, in pixels.
         */
        public void setZ(float z)
        {
            setTranslationZ(z - getElevation());
        }

        public float getElevation()
        {
            // NOP for now
            return 0;
        }

        /**
         * Sets the base elevation of this view, in pixels.
         *
         * @attr ref android.R.styleable#View_elevation
         */
        public void setElevation(float elevation)
        {
            // NOP for now
        }

        float tx;
        float ty;
        float tz;

        /**
         * The horizontal location of this view relative to its {@link #getLeft() left} position.
         * This position is post-layout, in addition to wherever the object's
         * layout placed it.
         *
         * @return The horizontal position of this view relative to its left position, in pixels.
         */
        public float getTranslationX()
        {
            return tz; // mRenderNode.getTranslationX();
        }

        /**
            * Sets the horizontal location of this view relative to its {@link #getLeft() left} position.
            * This effectively positions the object post-layout, in addition to wherever the object's
            * layout placed it.
            *
            * @param translationX The horizontal position of this view relative to its left position,
            * in pixels.
            *
            * @attr ref android.R.styleable#View_translationX
            */
        public void setTranslationX(float translationX)
        {
            if (translationX != getTranslationX())
            {
                tx = translationX;
                //invalidateViewProperty(true, false);
                //mRenderNode.setTranslationX(translationX);
                //invalidateViewProperty(false, true);

                //invalidateParentIfNeededAndWasQuickRejected();
                //notifySubtreeAccessibilityStateChangedIfNeeded();
            }
        }

        /**
         * The vertical location of this view relative to its {@link #getTop() top} position.
         * This position is post-layout, in addition to wherever the object's
         * layout placed it.
         *
         * @return The vertical position of this view relative to its top position,
         * in pixels.
         */
        public float getTranslationY()
        {
            return ty; // mRenderNode.getTranslationY();
        }

        /**
         * Sets the vertical location of this view relative to its {@link #getTop() top} position.
         * This effectively positions the object post-layout, in addition to wherever the object's
         * layout placed it.
         *
         * @param translationY The vertical position of this view relative to its top position,
         * in pixels.
         *
         * @attr ref android.R.styleable#View_translationY
         */
        public void setTranslationY(float translationY)
        {
            if (translationY != getTranslationY())
            {
                ty = translationY;
                //invalidateViewProperty(true, false);
                //mRenderNode.setTranslationY(translationY);
                //invalidateViewProperty(false, true);

                //invalidateParentIfNeededAndWasQuickRejected();
                //notifySubtreeAccessibilityStateChangedIfNeeded();
            }
        }

        /**
         * The depth location of this view relative to its {@link #getElevation() elevation}.
         *
         * @return The depth of this view relative to its elevation.
         */
        public float getTranslationZ()
        {
            return tz; // mRenderNode.getTranslationZ();
        }

        /**
         * Sets the depth location of this view relative to its {@link #getElevation() elevation}.
         *
         * @attr ref android.R.styleable#View_translationZ
         */
        public void setTranslationZ(float translationZ)
        {
            if (translationZ != getTranslationZ())
            {
                tz = translationZ;
                //translationZ = sanitizeFloatPropertyValue(translationZ, "translationZ");
                //invalidateViewProperty(true, false);
                //mRenderNode.setTranslationZ(translationZ);
                //invalidateViewProperty(false, true);

                //invalidateParentIfNeededAndWasQuickRejected();
            }
        }


        /**
         * Converts drawing order position to container position. Override this
         * if you want to change the drawing order of children. By default, it
         * returns drawingPosition.
         * <p>
         * NOTE: In order for this method to be called, you must enable child ordering
         * first by calling {@link #setChildrenDrawingOrderEnabled(bool)}.
         *
         * @param drawingPosition the drawing order position.
         * @return the container position of a child for this drawing order position.
         *
         * @see #setChildrenDrawingOrderEnabled(bool)
         * @see #isChildrenDrawingOrderEnabled()
         */
        protected int getChildDrawingOrder(int childCount, int drawingPosition)
        {
            return drawingPosition;
        }

        /**
         * Converts drawing order position to container position.
         * <p>
         * Children are not necessarily drawn in the order in which they appear in the container.
         * Views can enable a custom ordering via {@link #setChildrenDrawingOrderEnabled(bool)}.
         * This method returns the container position of a child that appears in the given position
         * in the current drawing order.
         *
         * @param drawingPosition the drawing order position.
         * @return the container position of a child for this drawing order position.
         *
         * @see #getChildDrawingOrder(int, int)}
         */
        public int getChildDrawingOrder(int drawingPosition)
        {
            return getChildDrawingOrder(getChildCount(), drawingPosition);
        }

        private bool hasChildWithZ()
        {
            for (int i = 0; i < mChildrenCount; i++)
            {
                if (mChildren[i].getZ() != 0) return true;
            }
            return false;
        }

        List<View> mPreSortedChildren;

        /**
         * Indicates whether the View is drawing its children in the order defined by
         * {@link #getChildDrawingOrder(int, int)}.
         *
         * @return true if children drawing order is defined by {@link #getChildDrawingOrder(int, int)},
         *         false otherwise
         *
         * @see #setChildrenDrawingOrderEnabled(bool)
         * @see #getChildDrawingOrder(int, int)
         */
        protected bool isChildrenDrawingOrderEnabled()
        {
            return (mGroupFlags & FLAG_USE_CHILD_DRAWING_ORDER) == FLAG_USE_CHILD_DRAWING_ORDER;
        }

        /**
         * Tells the View whether to draw its children in the order defined by the method
         * {@link #getChildDrawingOrder(int, int)}.
         * <p>
         * Note that {@link View#getZ() Z} reordering, done by {@link #dispatchDraw(Canvas)},
         * will override custom child ordering done via this method.
         *
         * @param enabled true if the order of the children when drawing is determined by
         *        {@link #getChildDrawingOrder(int, int)}, false otherwise
         *
         * @see #isChildrenDrawingOrderEnabled()
         * @see #getChildDrawingOrder(int, int)
         */
        protected void setChildrenDrawingOrderEnabled(bool enabled)
        {
            setboolFlag(FLAG_USE_CHILD_DRAWING_ORDER, enabled);
        }

        private bool hasboolFlag(int flag)
        {
            return (mGroupFlags & flag) == flag;
        }

        private void setboolFlag(int flag, bool value)
        {
            if (value)
            {
                mGroupFlags |= flag;
            }
            else
            {
                mGroupFlags &= ~flag;
            }
        }


        private int getAndVerifyPreorderedIndex(int childrenCount, int i, bool customOrder)
        {
            int childIndex;
            if (customOrder)
            {
                int childIndex1 = getChildDrawingOrder(childrenCount, i);
                if (childIndex1 >= childrenCount)
                {
                    throw new IndexOutOfRangeException("getChildDrawingOrder() "
                            + "returned invalid index " + childIndex1
                            + " (child count is " + childrenCount + ")");
                }
                childIndex = childIndex1;
            }
            else
            {
                childIndex = i;
            }
            return childIndex;
        }

        /**
         * Populates (and returns) mPreSortedChildren with a pre-ordered list of the View's children,
         * sorted first by Z, then by child drawing order (if applicable). This list must be cleared
         * after use to avoid leaking child Views.
         *
         * Uses a stable, insertion sort which is commonly O(n) for Views with very few elevated
         * children.
         */
        List<View> buildOrderedChildList()
        {
            int childrenCount = mChildrenCount;
            if (childrenCount <= 1 || !hasChildWithZ()) return null;

            if (mPreSortedChildren == null)
            {
                mPreSortedChildren = new(childrenCount);
            }
            else
            {
                // callers should clear, so clear shouldn't be necessary, but for safety...
                mPreSortedChildren.Clear();

                // no equvilant in C#  need to reallocate
                //mPreSortedChildren.EnsureCapacity(childrenCount);
                mPreSortedChildren = new(childrenCount);
            }

            bool customOrder = isChildrenDrawingOrderEnabled();
            for (int i = 0; i < childrenCount; i++)
            {
                // add next child (in child order) to end of list
                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                View nextChild = mChildren[childIndex];
                float currentZ = nextChild.getZ();

                // insert ahead of any Views with greater Z
                int insertIndex = i;
                while (insertIndex > 0 && mPreSortedChildren.ElementAt(insertIndex - 1).getZ() > currentZ)
                {
                    insertIndex--;
                }
                mPreSortedChildren[insertIndex] = nextChild;
            }
            return mPreSortedChildren;
        }


        private static View getAndVerifyPreorderedView(List<View> preorderedList, View[] children,
                int childIndex)
        {
            View child;
            if (preorderedList != null)
            {
                child = preorderedList.ElementAt(childIndex);
                if (child == null)
                {
                    throw new Exception("Invalid preorderedList contained null child at index "
                            + childIndex);
                }
            }
            else
            {
                child = children[childIndex];
            }
            return child;
        }

        class Transformation
        {
            internal void clear()
            {
                throw new NotImplementedException();
            }

            internal int getTransformationType()
            {
                throw new NotImplementedException();
            }
        }

        Transformation mChildTransformation;

        Transformation getChildTransformation()
        {
            if (mChildTransformation == null)
            {
                mChildTransformation = new Transformation();
            }
            return mChildTransformation;
        }

        /**
         * When set, this View supports static transformations on children; this causes
         * {@link #getChildStaticTransformation(View, android.view.animation.Transformation)} to be
         * invoked when a child is drawn.
         *
         * Any subclass overriding
         * {@link #getChildStaticTransformation(View, android.view.animation.Transformation)} should
         * set this flags in {@link #mGroupFlags}.
         *
         * {@hide}
         */
        internal const int FLAG_SUPPORT_STATIC_TRANSFORMATIONS = 0x800;

        /**
         * When this property is set to true, this View supports static transformations on
         * children; this causes
         * {@link #getChildStaticTransformation(View, android.view.animation.Transformation)} to be
         * invoked when a child is drawn.
         *
         * Any subclass overriding
         * {@link #getChildStaticTransformation(View, android.view.animation.Transformation)} should
         * set this property to true.
         *
         * @param enabled True to enable static transformations on children, false otherwise.
         *
         * @see #getChildStaticTransformation(View, android.view.animation.Transformation)
         */
        protected void setStaticTransformationsEnabled(bool enabled)
        {
            setboolFlag(FLAG_SUPPORT_STATIC_TRANSFORMATIONS, enabled);
        }

        /**
         * Sets  <code>t</code> to be the static transformation of the child, if set, returning a
         * bool to indicate whether a static transform was set. The default implementation
         * simply returns <code>false</code>; subclasses may override this method for different
         * behavior. {@link #setStaticTransformationsEnabled(bool)} must be set to true
         * for this method to be called.
         *
         * @param child The child view whose static transform is being requested
         * @param t The Transformation which will hold the result
         * @return true if the transformation was set, false otherwise
         * @see #setStaticTransformationsEnabled(bool)
         */
        bool getChildStaticTransformation(View child, Transformation t)
        {
            return false;
        }

        /**
         * This method is called by View.drawChild() to have each child view draw itself.
         *
         * This is where the View specializes rendering behavior based on layer type,
         * and hardware acceleration.
         */
        bool draw(SKCanvas canvas, View parent, long drawingTime)
        {
            bool hardwareAcceleratedCanvas = canvas.isHardwareAccelerated();
            /* If an attached view draws to a HW canvas, it may use its RenderNode + DisplayList.
             *
             * If a view is dettached, its DisplayList shouldn't exist. If the canvas isn't
             * HW accelerated, it can't handle drawing RenderNodes.
             */
            bool drawingWithRenderNode = mAttachInfo != null
                    && mAttachInfo.mHardwareAccelerated
                    && hardwareAcceleratedCanvas;

            bool more = false;
            bool childHasIdentityMatrix = hasIdentityMatrix();
            int parentFlags = parent.mGroupFlags;

            if ((parentFlags & FLAG_CLEAR_TRANSFORMATION) != 0)
            {
                parent.getChildTransformation().clear();
                parent.mGroupFlags &= ~FLAG_CLEAR_TRANSFORMATION;
            }

            Transformation transformToApply = null;
            bool concatMatrix = false;
            bool scalingRequired = mAttachInfo != null && mAttachInfo.mScalingRequired;
            //Animation a = getAnimation();
            //if (a != null)
            //{
            //    more = applyLegacyAnimation(parent, drawingTime, a, scalingRequired);
            //    concatMatrix = a.willChangeTransformationMatrix();
            //    if (concatMatrix)
            //    {
            //        mPrivateFlags3 |= PFLAG3_VIEW_IS_ANIMATING_TRANSFORM;
            //    }
            //    transformToApply = parent.getChildTransformation();
            //}
            //else
            //{
            //if ((mPrivateFlags3 & PFLAG3_VIEW_IS_ANIMATING_TRANSFORM) != 0)
            //{
            //    // No longer animating: clear out old animation matrix
            //    mRenderNode.setAnimationMatrix(null);
            //    mPrivateFlags3 &= ~PFLAG3_VIEW_IS_ANIMATING_TRANSFORM;
            //}
            if (!drawingWithRenderNode
                    && (parentFlags & FLAG_SUPPORT_STATIC_TRANSFORMATIONS) != 0)
            {
                Transformation t = parent.getChildTransformation();
                bool hasTransform = parent.getChildStaticTransformation(this, t);
                if (hasTransform)
                {
                    int transformType = t.getTransformationType();
                    transformToApply = null; //transformType != Transformation.TYPE_IDENTITY ? t : null;
                    concatMatrix = false; // (transformType & Transformation.TYPE_MATRIX) != 0;
                }
            }
            //}

            concatMatrix |= !childHasIdentityMatrix;

            // Sets the flag as early as possible to allow draw() implementations
            // to call invalidate() successfully when doing animations
            mPrivateFlags |= PFLAG_DRAWN;

            if (!concatMatrix &&
                    (parentFlags & (FLAG_SUPPORT_STATIC_TRANSFORMATIONS |
                            FLAG_CLIP_CHILDREN)) == FLAG_CLIP_CHILDREN &&
                    canvas.QuickReject(new SKRect(mLeft, mTop, mRight, mBottom))
                //&& (mPrivateFlags & PFLAG_DRAW_ANIMATION) == 0
                )
            {
                mPrivateFlags |= PFLAG_VIEW_QUICK_REJECTED;
                return more;
            }
            mPrivateFlags &= ~PFLAG_VIEW_QUICK_REJECTED;

            if (hardwareAcceleratedCanvas)
            {
                // Clear INVALIDATED flag to allow invalidation to occur during rendering, but
                // retain the flag's value temporarily in the mRecreateDisplayList flag
                mRecreateDisplayList = (mPrivateFlags & PFLAG_INVALIDATED) != 0;
                mPrivateFlags &= ~PFLAG_INVALIDATED;
            }

            SKPicture renderNode = null;
            SKBitmap cache = null;
            //int layerType = getLayerType(); // TODO: signify cache state with just 'cache' local
            //if (layerType == LAYER_TYPE_SOFTWARE || !drawingWithRenderNode)
            //{
            //    if (layerType != LAYER_TYPE_NONE)
            //    {
            //        // If not drawing with RenderNode, treat HW layers as SW
            //        layerType = LAYER_TYPE_SOFTWARE;
            //        buildDrawingCache(true);
            //    }
            //    cache = getDrawingCache(true);
            //}

            if (drawingWithRenderNode)
            {
                // Delay getting the display list until animation-driven alpha values are
                // set up and possibly passed on to the view
                renderNode = updateDisplayListIfDirty();
                //if (!renderNode.hasDisplayList())
                //{
                //    // Uncommon, but possible. If a view is removed from the hierarchy during the call
                //    // to getDisplayList(), the display list will be marked invalid and we should not
                //    // try to use it again.
                //    renderNode = null;
                //    drawingWithRenderNode = false;
                //}
            }

            int sx = 0;
            int sy = 0;
            if (!drawingWithRenderNode)
            {
                //computeScroll();
                sx = mScrollX;
                sy = mScrollY;
            }

            bool drawingWithDrawingCache = cache != null && !drawingWithRenderNode;
            bool offsetForScroll = cache == null && !drawingWithRenderNode;

            int restoreTo = -1;
            if (!drawingWithRenderNode || transformToApply != null)
            {
                restoreTo = canvas.Save();
            }
            if (offsetForScroll)
            {
                canvas.Translate(mLeft - sx, mTop - sy);
            }
            else
            {
                if (!drawingWithRenderNode)
                {
                    canvas.Translate(mLeft, mTop);
                }
                if (scalingRequired)
                {
                    if (drawingWithRenderNode)
                    {
                        // TODO: Might not need this if we put everything inside the DL
                        restoreTo = canvas.Save();
                    }
                    // mAttachInfo cannot be null, otherwise scalingRequired == false
                    float scale = 1.0f / mAttachInfo.mApplicationScale;
                    canvas.Scale(scale, scale);
                }
            }

            float alpha = 1.0f; // drawingWithRenderNode ? 1 : (getAlpha() * getTransitionAlpha());
            if (transformToApply != null
                    || alpha < 1
                    || !hasIdentityMatrix()
                //|| (mPrivateFlags3 & PFLAG3_VIEW_IS_ANIMATING_ALPHA) != 0
                )
            {
                if (transformToApply != null || !childHasIdentityMatrix)
                {
                    int transX = 0;
                    int transY = 0;

                    if (offsetForScroll)
                    {
                        transX = -sx;
                        transY = -sy;
                    }

                    if (transformToApply != null)
                    {
                        if (concatMatrix)
                        {
                            if (drawingWithRenderNode)
                            {
                                //renderNode.setAnimationMatrix(transformToApply.getMatrix());
                            }
                            else
                            {
                                // Undo the scroll translation, apply the transformation matrix,
                                // then redo the scroll translate to get the correct result.
                                canvas.Translate(-transX, -transY);
                                //canvas.Concat(transformToApply.getMatrix());
                                canvas.Translate(transX, transY);
                            }
                            parent.mGroupFlags |= FLAG_CLEAR_TRANSFORMATION;
                        }

                        float transformAlpha = 1.0f; //transformToApply.getAlpha();
                        if (transformAlpha < 1)
                        {
                            alpha *= transformAlpha;
                            parent.mGroupFlags |= FLAG_CLEAR_TRANSFORMATION;
                        }
                    }

                    if (!childHasIdentityMatrix && !drawingWithRenderNode)
                    {
                        canvas.Translate(-transX, -transY);
                        //canvas.concat(getMatrix());
                        canvas.Translate(transX, transY);
                    }
                }

                // Deal with alpha if it is or used to be <1
                if (alpha < 1
                    //|| (mPrivateFlags3 & PFLAG3_VIEW_IS_ANIMATING_ALPHA) != 0
                    )
                {
                    if (alpha < 1)
                    {
                        //mPrivateFlags3 |= PFLAG3_VIEW_IS_ANIMATING_ALPHA;
                    }
                    else
                    {
                        //mPrivateFlags3 &= ~PFLAG3_VIEW_IS_ANIMATING_ALPHA;
                    }
                    parent.mGroupFlags |= FLAG_CLEAR_TRANSFORMATION;
                    if (!drawingWithDrawingCache)
                    {
                        //int multipliedAlpha = (int)(255 * alpha);
                        //if (!onSetAlpha(multipliedAlpha))
                        //{
                        //    if (drawingWithRenderNode)
                        //    {
                        //        renderNode.setAlpha(alpha * getAlpha() * getTransitionAlpha());
                        //    }
                        //    else if (layerType == LAYER_TYPE_NONE)
                        //    {
                        //        canvas.saveLayerAlpha(sx, sy, sx + getWidth(), sy + getHeight(),
                        //                multipliedAlpha);
                        //    }
                        //}
                        //else
                        //{
                        //    // Alpha is handled by the child directly, clobber the layer's alpha
                        //    mPrivateFlags |= PFLAG_ALPHA_SET;
                        //}
                    }
                }
            }
            //else if ((mPrivateFlags & PFLAG_ALPHA_SET) == PFLAG_ALPHA_SET)
            //{
            //    onSetAlpha(255);
            //    mPrivateFlags &= ~PFLAG_ALPHA_SET;
            //}

            if (!drawingWithRenderNode)
            {
                // apply clips directly, since RenderNode won't do it for this draw
                if ((parentFlags & FLAG_CLIP_CHILDREN) != 0 && cache == null)
                {
                    if (offsetForScroll)
                    {
                        canvas.ClipRect(sx, sy, sx + getWidth(), sy + getHeight());
                    }
                    else
                    {
                        if (!scalingRequired || cache == null)
                        {
                            canvas.ClipRect(0, 0, getWidth(), getHeight());
                        }
                        else
                        {
                            canvas.ClipRect(0, 0, cache.Width, cache.Height);
                        }
                    }
                }

                //if (mClipBounds != null)
                //{
                //    clip bounds ignore scroll
                //    canvas.clipRect(mClipBounds);
                //}
            }

            if (!drawingWithDrawingCache)
            {
                if (drawingWithRenderNode)
                {
                    mPrivateFlags &= ~PFLAG_DIRTY_MASK;
                    //((RecordingCanvas)canvas).drawRenderNode(renderNode);
                    canvas.DrawPicture(renderNode, 0, 0);
                }
                else
                {
                    // Fast path for layouts with no backgrounds
                    if ((mPrivateFlags & PFLAG_SKIP_DRAW) == PFLAG_SKIP_DRAW)
                    {
                        mPrivateFlags &= ~PFLAG_DIRTY_MASK;
                        dispatchDraw(canvas);
                    }
                    else
                    {
                        draw(canvas);
                    }
                }
            }
            else if (cache != null)
            {
                mPrivateFlags &= ~PFLAG_DIRTY_MASK;
                //if (
                //    //layerType == LAYER_TYPE_NONE
                //    //|| mLayerPaint == null
                //)
                //{
                //    // no layer paint, use temporary paint to draw bitmap
                //    Paint cachePaint = parent.mCachePaint;
                //    if (cachePaint == null)
                //    {
                //        cachePaint = new Paint();
                //        cachePaint.setDither(false);
                //        parent.mCachePaint = cachePaint;
                //    }
                //    cachePaint.setAlpha((int)(alpha * 255));
                //    canvas.drawBitmap(cache, 0.0f, 0.0f, cachePaint);
                //}
                //else
                //{
                //    // use layer paint to draw the bitmap, merging the two alphas, but also restore
                //    int layerPaintAlpha = mLayerPaint.getAlpha();
                //    if (alpha < 1)
                //    {
                //        mLayerPaint.setAlpha((int)(alpha * layerPaintAlpha));
                //    }
                //    canvas.drawBitmap(cache, 0.0f, 0.0f, mLayerPaint);
                //    if (alpha < 1)
                //    {
                //        mLayerPaint.setAlpha(layerPaintAlpha);
                //    }
                //}
            }

            if (restoreTo >= 0)
            {
                canvas.RestoreToCount(restoreTo);
            }

            //if (a != null && !more)
            //{
            //    if (!hardwareAcceleratedCanvas && !a.getFillAfter())
            //    {
            //        onSetAlpha(255);
            //    }
            //    parent.finishAnimatingView(this, a);
            //}

            if (more && hardwareAcceleratedCanvas)
            {
                //if (a.hasAlpha() && (mPrivateFlags & PFLAG_ALPHA_SET) == PFLAG_ALPHA_SET)
                //{
                //    // alpha animations should cause the child to recreate its display list
                //    invalidate(true);
                //}
            }

            mRecreateDisplayList = false;

            return more;
        }

        /**
         * Draw one child of this View Group. This method is responsible for getting
         * the canvas in the right state. This includes clipping, translating so
         * that the child's scrolled origin is at 0, 0, and applying any animation
         * transformations.
         *
         * @param canvas The canvas on which to draw the child
         * @param child Who to draw
         * @param drawingTime The time at which draw is occurring
         * @return True if an invalidate() was issued
         */
        protected bool drawChild(SKCanvas canvas, View child, long drawingTime)
        {
            return child.draw(canvas, this, drawingTime);
        }


        /**
         * Called by draw to draw the child views. This may be overridden
         * by derived classes to gain control just before its children are drawn
         * (but after its own view has been drawn).
         * @param canvas the canvas on which to draw the view
         */
        protected void dispatchDraw(SKCanvas canvas)
        {
            int childrenCount = mChildrenCount;
            View[] children = mChildren;
            int flags = mGroupFlags;

            //if ((flags & FLAG_RUN_ANIMATION) != 0 && canAnimate()) {
            //    for (int i = 0; i < childrenCount; i++) {
            //        View child = children[i];
            //        if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE) {
            //            LayoutParams params = child.getLayoutParams();
            //            attachLayoutAnimationParameters(child, params, i, childrenCount);
            //            bindLayoutAnimation(child);
            //        }
            //    }

            //    LayoutAnimationController controller = mLayoutAnimationController;
            //    if (controller.willOverlap()) {
            //        mGroupFlags |= FLAG_OPTIMIZE_INVALIDATE;
            //    }

            //    controller.start();

            //    mGroupFlags &= ~FLAG_RUN_ANIMATION;
            //    mGroupFlags &= ~FLAG_ANIMATION_DONE;

            //    if (mAnimationListener != null) {
            //        mAnimationListener.onAnimationStart(controller.getAnimation());
            //    }
            //}

            int clipSaveCount = 0;
            bool clipToPadding = (flags & CLIP_TO_PADDING_MASK) == CLIP_TO_PADDING_MASK;
            if (clipToPadding)
            {
                clipSaveCount = canvas.Save();
                canvas.ClipRect(mScrollX + mPaddingLeft, mScrollY + mPaddingTop,
                        mScrollX + mRight - mLeft - mPaddingRight,
                        mScrollY + mBottom - mTop - mPaddingBottom);
            }

            // We will draw our child's animation, let's reset the flag
            //mPrivateFlags &= ~PFLAG_DRAW_ANIMATION;
            mGroupFlags &= ~FLAG_INVALIDATE_REQUIRED;

            bool more = false;
            long drawingTime = getDrawingTime();

            // Z not supported
            //canvas.EnableZ();

            int transientCount = 0; // mTransientIndices == null ? 0 : mTransientIndices.size();
            int transientIndex = transientCount != 0 ? 0 : -1;
            // Only use the preordered list if not HW accelerated, since the HW pipeline will do the
            // draw reordering internally
            List<View> preorderedList = isHardwareAccelerated()
                    ? null : buildOrderedChildList();
            bool customOrder = preorderedList == null
                    && isChildrenDrawingOrderEnabled();
            for (int i = 0; i < childrenCount; i++)
            {
                //while (transientIndex >= 0 && mTransientIndices.get(transientIndex) == i)
                //{
                //    View transientChild = mTransientViews.get(transientIndex);
                //    if ((transientChild.mViewFlags & VISIBILITY_MASK) == VISIBLE ||
                //            transientChild.getAnimation() != null)
                //    {
                //        more |= drawChild(canvas, transientChild, drawingTime);
                //    }
                //    transientIndex++;
                //    if (transientIndex >= transientCount)
                //    {
                //        transientIndex = -1;
                //    }
                //}

                int childIndex = getAndVerifyPreorderedIndex(childrenCount, i, customOrder);
                View child = getAndVerifyPreorderedView(preorderedList, children, childIndex);
                if ((child.mViewFlags & VISIBILITY_MASK) == VISIBLE
                    //|| child.getAnimation() != null
                    )
                {
                    more |= drawChild(canvas, child, drawingTime);
                }
            }
            //while (transientIndex >= 0)
            //{
            //    // there may be additional transient views after the normal views
            //    View transientChild = mTransientViews.get(transientIndex);
            //    if ((transientChild.mViewFlags & VISIBILITY_MASK) == VISIBLE ||
            //            transientChild.getAnimation() != null)
            //    {
            //        more |= drawChild(canvas, transientChild, drawingTime);
            //    }
            //    transientIndex++;
            //    if (transientIndex >= transientCount)
            //    {
            //        break;
            //    }
            //}
            if (preorderedList != null) preorderedList.Clear();

            // Draw any disappearing views that have animations
            //if (mDisappearingChildren != null)
            //{
            //    ArrayList<View> disappearingChildren = mDisappearingChildren;
            //    int disappearingCount = disappearingChildren.size() - 1;
            //    // Go backwards -- we may delete as animations finish
            //    for (int i = disappearingCount; i >= 0; i--)
            //    {
            //        View child = disappearingChildren.get(i);
            //        more |= drawChild(canvas, child, drawingTime);
            //    }
            //}

            // no Z support
            //canvas.disableZ();

            if (isShowingLayoutBounds())
            {
                onDebugDraw(canvas);
            }

            if (clipToPadding)
            {
                canvas.RestoreToCount(clipSaveCount);
            }

            // mGroupFlags might have been updated by drawChild()
            flags = mGroupFlags;

            if ((flags & FLAG_INVALIDATE_REQUIRED) == FLAG_INVALIDATE_REQUIRED)
            {
                invalidate(true);
            }

            //if ((flags & FLAG_ANIMATION_DONE) == 0 && (flags & FLAG_NOTIFY_ANIMATION_LISTENER) == 0 &&
            //        mLayoutAnimationController.isDone() && !more) {
            //    // We want to erase the drawing cache and notify the listener after the
            //    // next frame is drawn because one extra invalidate() is caused by
            //    // drawChild() after the animation is over
            //    mGroupFlags |= FLAG_NOTIFY_ANIMATION_LISTENER;
            //    Runnable end = new Runnable() {
            //       @Override
            //       public void run() {
            //           notifyAnimationListener();
            //       }
            //    };
            //    post(end);
            //}
        }



        int getId() => NO_ID;

        /**
         * The view's tag.
         * {@hide}
         *
         * @see #setTag(Object)
         * @see #getTag()
         */
        protected object mTag = null;

        /**
         * Returns this view's tag.
         *
         * @return the Object stored in this view as a tag, or {@code null} if not
         *         set
         *
         * @see #setTag(Object)
         * @see #getTag(int)
         */
        public object getTag()
        {
            return mTag;
        }

        /**
         * Sets the tag associated with this view. A tag can be used to mark
         * a view in its hierarchy and does not have to be unique within the
         * hierarchy. Tags can also be used to store data within a view without
         * resorting to another data structure.
         *
         * @param tag an Object to tag the view with
         *
         * @see #getTag()
         * @see #setTag(int, Object)
         */
        public void setTag(object tag)
        {
            mTag = tag;
        }

        private const bool DBG = true;

        /**
         * The logging tag used by this class with android.util.Log.
         */
        protected const string VIEW_LOG_TAG = "View";

        /**
         * Build a human readable string representation of the specified view flags.
         *
         * @param flags the view flags to convert to a string
         * @return a String representing the supplied flags
         */
        private static string printFlags(int flags)
        {
            string output = "";
            int numFlags = 0;
            if ((flags & FOCUSABLE) == FOCUSABLE)
            {
                output += "TAKES_FOCUS";
                numFlags++;
            }

            switch (flags & VISIBILITY_MASK)
            {
                case INVISIBLE:
                    if (numFlags > 0)
                    {
                        output += " ";
                    }
                    output += "INVISIBLE";
                    // USELESS HERE numFlags++;
                    break;
                case GONE:
                    if (numFlags > 0)
                    {
                        output += " ";
                    }
                    output += "GONE";
                    // USELESS HERE numFlags++;
                    break;
                default:
                    break;
            }
            return output;
        }

        /**
         * Build a human readable string representation of the specified private
         * view flags.
         *
         * @param privateFlags the private view flags to convert to a string
         * @return a String representing the supplied flags
         */
        private static string printPrivateFlags(int privateFlags)
        {
            string output = "";
            int numFlags = 0;

            if ((privateFlags & PFLAG_WANTS_FOCUS) == PFLAG_WANTS_FOCUS)
            {
                output += "WANTS_FOCUS";
                numFlags++;
            }

            if ((privateFlags & PFLAG_FOCUSED) == PFLAG_FOCUSED)
            {
                if (numFlags > 0)
                {
                    output += " ";
                }
                output += "FOCUSED";
                numFlags++;
            }

            //if ((privateFlags & PFLAG_SELECTED) == PFLAG_SELECTED)
            //{
            //    if (numFlags > 0)
            //    {
            //        output += " ";
            //    }
            //    output += "SELECTED";
            //    numFlags++;
            //}

            if ((privateFlags & PFLAG_IS_ROOT_NAMESPACE) == PFLAG_IS_ROOT_NAMESPACE)
            {
                if (numFlags > 0)
                {
                    output += " ";
                }
                output += "IS_ROOT_NAMESPACE";
                numFlags++;
            }

            if ((privateFlags & PFLAG_HAS_BOUNDS) == PFLAG_HAS_BOUNDS)
            {
                if (numFlags > 0)
                {
                    output += " ";
                }
                output += "HAS_BOUNDS";
                numFlags++;
            }

            if ((privateFlags & PFLAG_DRAWN) == PFLAG_DRAWN)
            {
                if (numFlags > 0)
                {
                    output += " ";
                }
                output += "DRAWN";
                // USELESS HERE numFlags++;
            }
            return output;
        }

        /**
         * Prints information about this view in the log output, with the tag
         * {@link #VIEW_LOG_TAG}.
         *
         * @hide
         */
        public void debug()
        {
            debug(0);
        }

        /**
         * Prints information about this view in the log output, with the tag
         * {@link #VIEW_LOG_TAG}. Each line in the output is preceded with an
         * indentation defined by the <code>depth</code>.
         *
         * @param depth the indentation level
         *
         * @hide
         */
        protected void debug(int depth)
        {
            string output = debugIndent(depth - 1);

            output += "+ " + this;
            int id = getId();
            if (id != -1)
            {
                output += " (id=" + id + ")";
            }
            object tag = getTag();
            if (tag != null)
            {
                output += " (tag=" + tag + ")";
            }
            Log.d(VIEW_LOG_TAG, output);

            if ((mPrivateFlags & PFLAG_FOCUSED) != 0)
            {
                output = debugIndent(depth) + " FOCUSED";
                Log.d(VIEW_LOG_TAG, output);
            }

            output = debugIndent(depth);
            output += "frame={" + mLeft + ", " + mTop + ", " + mRight
                    + ", " + mBottom + "} scroll={" + mScrollX + ", " + mScrollY
                    + "} ";
            Log.d(VIEW_LOG_TAG, output);

            if (mPaddingLeft != 0 || mPaddingTop != 0 || mPaddingRight != 0
                    || mPaddingBottom != 0)
            {
                output = debugIndent(depth);
                output += "padding={" + mPaddingLeft + ", " + mPaddingTop
                        + ", " + mPaddingRight + ", " + mPaddingBottom + "}";
                Log.d(VIEW_LOG_TAG, output);
            }

            output = debugIndent(depth);
            output += "mMeasureWidth=" + mMeasuredWidth +
                    " mMeasureHeight=" + mMeasuredHeight;
            Log.d(VIEW_LOG_TAG, output);

            output = debugIndent(depth);
            if (mLayoutParams == null)
            {
                output += "BAD! no layout params";
            }
            else
            {
                output = mLayoutParams.debug(output);
            }
            Log.d(VIEW_LOG_TAG, output);

            output = debugIndent(depth);
            output += "flags={";
            output += View.printFlags(mViewFlags);
            output += "}";
            Log.d(VIEW_LOG_TAG, output);

            output = debugIndent(depth);
            output += "privateFlags={";
            output += View.printPrivateFlags(mPrivateFlags);
            output += "}";
            Log.d(VIEW_LOG_TAG, output);

            output = debugIndent(depth);
            output += "willDraw=" + willDraw();
            Log.d(VIEW_LOG_TAG, output);

            output = debugIndent(depth);
            output += "children={";

            int childCount = getChildCount();

            if (childCount > 0)
            {

                Log.d(VIEW_LOG_TAG, output);

                for (int i = 0; i < childCount; i++)
                {
                    View c = getChildAt(i);
                    c.debug(depth + 2);
                }

                output = debugIndent(depth);
            }

            output += "}";
            Log.d(VIEW_LOG_TAG, output);
        }

        /**
         * Creates a string of whitespaces used for indentation.
         *
         * @param depth the indentation level
         * @return a String containing (depth * 2 + 3) * 2 white spaces
         *
         * @hide
         */
        protected static string debugIndent(int depth)
        {
            System.Text.StringBuilder spaces = new System.Text.StringBuilder((depth * 2 + 3) * 2);
            for (int i = 0; i < (depth * 2) + 3; i++)
            {
                spaces.Append(' ').Append(' ');
            }
            return spaces.ToString();
        }

        /**
         * <p>Return the offset of the widget's text baseline from the widget's top
         * boundary. If this widget does not support baseline alignment, this
         * method returns -1. </p>
         *
         * @return the offset of the baseline within the widget's bounds or -1
         *         if baseline alignment is not supported
         */
        public int getBaseline()
        {
            return -1;
        }

        /**
         * Returns whether the view hierarchy is currently undergoing a layout pass. This
         * information is useful to avoid situations such as calling {@link #requestLayout()} during
         * a layout pass.
         *
         * @return whether the view hierarchy is currently undergoing a layout pass
         */
        public bool isInLayout()
        {
            ViewRootImpl viewRoot = getViewRootImpl();
            return (viewRoot != null && viewRoot.isInLayout());
        }

        /**
         * Call this when something has changed which has invalidated the
         * layout of this view. This will schedule a layout pass of the view
         * tree. This should not be called while the view hierarchy is currently in a layout
         * pass ({@link #isInLayout()}. If layout is happening, the request may be honored at the
         * end of the current layout pass (and then layout will run again) or after the current
         * frame is drawn and the next layout occurs.
         *
         * <p>Subclasses which override this method should call the superclass method to
         * handle possible request-during-layout errors correctly.</p>
         */
        public void requestLayout()
        {
            if (mMeasureCache != null) mMeasureCache.clear();

            if (mAttachInfo != null && mAttachInfo.mViewRequestingLayout == null)
            {
                // Only trigger request-during-layout logic if this is the view requesting it,
                // not the views in its parent hierarchy
                ViewRootImpl viewRoot = getViewRootImpl();
                if (viewRoot != null && viewRoot.isInLayout())
                {
                    if (!viewRoot.requestLayoutDuringLayout(this))
                    {
                        return;
                    }
                }
                mAttachInfo.mViewRequestingLayout = this;
            }

            mPrivateFlags |= PFLAG_FORCE_LAYOUT;
            mPrivateFlags |= PFLAG_INVALIDATED;

            if (mParent != null && !mParent.isLayoutRequested())
            {
                mParent.requestLayout();
            }
            if (mAttachInfo != null && mAttachInfo.mViewRequestingLayout == this)
            {
                mAttachInfo.mViewRequestingLayout = null;
            }
        }

        /**
         * <p>Indicates whether or not this view's layout will be requested during
         * the next hierarchy layout pass.</p>
         *
         * @return true if the layout will be forced during next layout pass
         */
        public bool isLayoutRequested()
        {
            return (mPrivateFlags & PFLAG_FORCE_LAYOUT) == PFLAG_FORCE_LAYOUT;
        }
    }
}
