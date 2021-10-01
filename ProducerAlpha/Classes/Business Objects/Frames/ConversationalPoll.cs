using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;


namespace ProducerAlpha
{
    public class ConversationalPoll : Frame, IAcceptsBackgroundImage
    {
        //------------------------------------------------------------------------------------
        // Constructor Methods Section
        //------------------------------------------------------------------------------------
        public ConversationalPoll(Script pScript)
            : base(pScript)
        { }

        //------------------------------------------------------------------------------------
        public ConversationalPoll() : base() { }

        //------------------------------------------------------------------------------------
        // IAcceptsBackgroundImage Interface Implementation Section
        //------------------------------------------------------------------------------------
        // Private Member
        private String backgroundImageUri;
        private String inactiveBackgroundImageUri;

        //------------------------------------------------------------------------------------
        // Public Property
        [Browsable(false)]
        public String BackgroundImagePath
        {
            get
            {
                return this.backgroundImageUri;
            }
            set
            {
                this.backgroundImageUri = value;
                this.Script.ImageUrl = value;
            }
        }

        //------------------------------------------------------------------------------------
        [Browsable(false)]
        public String InactiveBackgroundImagePath
        {
            get
            {
                return this.inactiveBackgroundImageUri;
            }
            set
            {
                this.inactiveBackgroundImageUri = value;
            }
        }


    }
}
