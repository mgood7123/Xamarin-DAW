namespace Xamarin_DAW.Skia_UI_Kit
{
    // this does nothing, only here to link classes together
    public interface Parent
    {
        public void clearChildFocus(View child) { }

        /**
         * Returns the parent if it exists, or null.
         *
         * @return a ViewParent or null if this ViewParent does not have a parent
         */
        abstract public Parent getParent();

        /**
         * Unbuffered dispatch has been requested by a child of this view parent.
         * This method is called by the View hierarchy to signal ancestors that a View needs to
         * request unbuffered dispatch.
         *
         * @see View#requestUnbufferedDispatch(int)
         * @hide
         */
        void onDescendantUnbufferedRequested()
        {
            if (getParent() != null)
            {
                getParent().onDescendantUnbufferedRequested();
            }
        }

        void requestChildFocus(View view1, View view2);
        View focusSearch(View view, int direction);
        void focusableViewAvailable(View view);
        bool isLayoutRequested();
        void requestLayout();
        void invalidate();
        bool canResolveLayoutDirection() { return false; }

        bool isLayoutDirectionResolved();
        int getLayoutDirection();
    }
}